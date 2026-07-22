using Microsoft.EntityFrameworkCore;
using StudyBuddy.DAL.Context;
using StudyBuddy.DAL.Repositories.Interfaces;
using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}