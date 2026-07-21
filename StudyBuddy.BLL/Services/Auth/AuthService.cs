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
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ITokenService _tokenService;
        public AuthService(IAuthRepository repository, ITokenService tokenService)
        {
            _authRepository = repository;
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
                return (new LoginResponceDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Token = _tokenService.GenerateToken(user)
                });
            }

            var userbymail = await _authRepository.GetUserByEmailAsync(email);

            if (userbymail != null)
            {
                // Link Google account with existing account
                userbymail.GoogleId = googleId;
                userbymail.AuthenticationProvider = "Google";

                await _authRepository.UpdateUserAsync(userbymail);

                return new LoginResponceDto
                {
                    Id = userbymail.Id,
                    FullName = userbymail.FullName,
                    Email = userbymail.Email,
                    Token = _tokenService.GenerateToken(userbymail)
                };
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

            return new LoginResponceDto
            {
                Id = newUser.Id,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Token = _tokenService.GenerateToken(newUser)
            };
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
            return (new LoginResponceDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = _tokenService.GenerateToken(user)
            });
        }
    }
}


// Flow -> Auth + Password ->. object creation by dependcy injection.
// constructor injection -> RegisterAsync method -> check if email exists -> if exists throw exception -> create new user object -> hash password -> save user to repository -> return response DTO with user info. 
// The AuthService class is responsible for handling user registration logic. It checks if the email already exists in the database, hashes the user's password, and saves the new user to the repository. If the email already exists, it throws an exception. The service returns a response DTO containing the newly created user's information.