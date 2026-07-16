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
    }
}
