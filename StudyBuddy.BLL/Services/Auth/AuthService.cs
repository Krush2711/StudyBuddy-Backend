using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using StudyBuddy.BLL.DTOs.Auths;
using StudyBuddy.BLL.Exceptions;
using StudyBuddy.BLL.Interfaces.Auth;
using StudyBuddy.DAL.Interfaces.Auth;
using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;



namespace StudyBuddy.BLL.Services.Add
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        
        public AuthService(IAuthRepository repository)
        {
            _authRepository = repository;
            _passwordHasher = new PasswordHasher<User>();
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
    }
}


// Flow -> Auth + Password ->. object creation by dependcy injection.
// constructor injection -> RegisterAsync method -> check if email exists -> if exists throw exception -> create new user object -> hash password -> save user to repository -> return response DTO with user info. 
// The AuthService class is responsible for handling user registration logic. It checks if the email already exists in the database, hashes the user's password, and saves the new user to the repository. If the email already exists, it throws an exception. The service returns a response DTO containing the newly created user's information.