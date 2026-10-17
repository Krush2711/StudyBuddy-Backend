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

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RawDocument>> GetRawDocumentsByUserAsync(Guid userId)
        {
            return await _context.RawDocuments
                .AsNoTracking()
                .Include(x => x.StoredFile)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.RawDocuments)
                .ThenInclude(x => x.StoredFile)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return false;
            }

            var storedFiles = user.RawDocuments
                .Where(x => x.StoredFile != null)
                .Select(x => x.StoredFile)
                .Distinct()
                .ToList();

            _context.Users.Remove(user);

            if (storedFiles.Count > 0)
            {
                _context.StoredFiles.RemoveRange(storedFiles);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
