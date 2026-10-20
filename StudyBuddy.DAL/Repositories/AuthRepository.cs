using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.DAL.Context;
using StudyBuddy.DAL.Interfaces.Auth;
using StudyBuddy.Models.Entities;



namespace StudyBuddy.DAL.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context; // filed -> "The repository always needs a database connection."
        public AuthRepository(AppDbContext context) { 
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email); // Does any user exist with this email
        }

        public async Task <User> CreateUSerAsync(User user)
        {
            await _context.Users.AddAsync(user); //Track this object. I want to insert it.
            await _context.SaveChangesAsync(); // This is where the database is actually updated.
            return user;
        }

        public async Task<User?> GetUserByGoogleIdAsync(string googleId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email); // getting user 
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<bool> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
