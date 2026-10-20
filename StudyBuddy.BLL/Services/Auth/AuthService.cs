using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StudyBuddy.BLL.DTOs.Auths;
using StudyBuddy.BLL.DTOs.Auths.Login;
using StudyBuddy.BLL.Exceptions;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.DAL.Interfaces.Auth;
using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using System.Text;




namespace StudyBuddy.BLL.Services.Add
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;
        public AuthService(
            IAuthRepository repository,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService)
        {
            _authRepository = repository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = new PasswordHasher<User>();
            _tokenService = tokenService;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var emailExists = await _authRepository.EmailExistsAsync(request.Email);

            if (emailExists)
            {
                throw new DuplicateEmailException(request.Email);
            }

            var user = new User // object maping.
            {
                FullName = request.FullName,
                Email = request.Email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            var createdUser = await _authRepository.CreateUSerAsync(user);

            return new RegisterResponseDto
            {
                Id = createdUser.Id,
                FullName = createdUser.FullName,
                Email = createdUser.Email
            };


        }

        public async Task<LoginResponceDto> GoogleLoginAsync(string googleId, string email, string name)
        {
            var user = await _authRepository.GetUserByGoogleIdAsync(googleId);

            if (user != null)
            {
                return await IssueSessionAsync(user);
            }

            var userbymail = await _authRepository.GetUserByEmailAsync(email);

            if (userbymail != null)
            {
                // Link Google account with existing account
                userbymail.GoogleId = googleId;
                userbymail.AuthenticationProvider = "Google";

                await _authRepository.UpdateUserAsync(userbymail);

                return await IssueSessionAsync(userbymail);
            }

            // First time Google login - create a new user
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = name,
                Email = email,
                GoogleId = googleId,
                AuthenticationProvider = "Google",
                PasswordHash = null,
                Isactive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.CreateUSerAsync(newUser);

            return await IssueSessionAsync(newUser);
        }

        public async Task<LoginResponceDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if ( user == null)
            {
                  throw new InvalidCredentialsException();
            }

            //bool isPasswordValid = _passwordHasher.VerifyHashedPassword(User, User.PasswordHash, request.Password) == PasswordVerificationResult.Success;
            var result = _passwordHasher.VerifyHashedPassword(
                            user,
                            user.PasswordHash,
                            request.Password);

            if (result != PasswordVerificationResult.Success)
            {
                throw new InvalidCredentialsException();
            }

            return await IssueSessionAsync(user);
        }

        /// <summary>
        /// Rotates a refresh token into a fresh access/refresh pair.
        ///
        /// Presenting an already-used token is treated as a leak: the whole
        /// family is revoked rather than silently re-issued, so a stolen token
        /// cannot be used alongside the legitimate session indefinitely.
        /// </summary>
        public async Task<LoginResponceDto> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new InvalidRefreshTokenException();
            }

            var hash = _tokenService.HashRefreshToken(refreshToken);

            var stored = await _refreshTokenRepository.GetByHashAsync(hash);

            if (stored == null)
            {
                throw new InvalidRefreshTokenException();
            }

            if (stored.RevokedAt != null)
            {
                // Replay of a token we already rotated or revoked.
                await _refreshTokenRepository.RevokeAllForUserAsync(
                    stored.UserId,
                    "replay-detected");
                throw new InvalidRefreshTokenException();
            }

            if (stored.ExpiresAt <= DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException();
            }

            var user = await _authRepository.GetUserByIdAsync(stored.UserId);

            if (user == null || !user.Isactive)
            {
                await _refreshTokenRepository.RevokeAllForUserAsync(
                    stored.UserId,
                    "user-inactive");
                throw new InvalidRefreshTokenException();
            }

            var session = await IssueSessionAsync(user);

            // Link the token we just consumed to its replacement, then revoke it.
            var replacementHash = _tokenService.HashRefreshToken(session.RefreshToken);
            var replacement = await _refreshTokenRepository.GetByHashAsync(replacementHash);

            stored.RevokedAt = DateTime.UtcNow;
            stored.RevokedReason = "rotated";
            stored.ReplacedByTokenId = replacement?.Id;

            await _refreshTokenRepository.UpdateAsync(stored);

            return session;
        }

        public async Task LogoutAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return;
            }

            var stored = await _refreshTokenRepository.GetByHashAsync(
                _tokenService.HashRefreshToken(refreshToken));

            // Idempotent: an unknown or already-revoked token is not an error.
            if (stored == null || stored.RevokedAt != null)
            {
                return;
            }

            stored.RevokedAt = DateTime.UtcNow;
            stored.RevokedReason = "logout";

            await _refreshTokenRepository.UpdateAsync(stored);
        }

        /// <summary>
        /// Mints an access token plus a brand-new refresh token for a user.
        /// Every login of any kind ends up here.
        /// </summary>
        private async Task<LoginResponceDto> IssueSessionAsync(User user)
        {
            var accessToken = _tokenService.GenerateToken(user);

            var secrets = await IssueRefreshTokenAsync(user.Id);

            return new LoginResponceDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = accessToken,
                RefreshToken = secrets.RawToken,
                ExpiresIn = _tokenService.AccessTokenExpiresInSeconds
            };
        }

        private async Task<(Guid Id, string RawToken)> IssueRefreshTokenAsync(Guid userId)
        {
            var rawToken = _tokenService.GenerateRefreshToken();

            var entity = new RefreshToken
            {
                UserId = userId,
                TokenHash = _tokenService.HashRefreshToken(rawToken),
                ExpiresAt = _tokenService.GetRefreshTokenExpiryUtc(),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(entity);

            return (entity.Id, rawToken);
        }
    }
}


// Flow -> Auth + Password ->. object creation by dependcy injection.
// constructor injection -> RegisterAsync method -> check if email exists -> if exists throw exception -> create new user object -> hash password -> save user to repository -> return response DTO with user info. 
// The AuthService class is responsible for handling user registration logic. It checks if the email already exists in the database, hashes the user's password, and saves the new user to the repository. If the email already exists, it throws an exception. The service returns a response DTO containing the newly created user's information.