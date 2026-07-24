using StudyBuddy.DAL.Context;
using StudyBuddy.DAL.Interfaces;
using StudyBuddy.Models.Entities;

namespace StudyBuddy.DAL.Repositories
{
    public class StoredFileRepository : IStoredFileRepository
    {
        private readonly AppDbContext _context;

        public StoredFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StoredFile> AddAsync(StoredFile storedFile)
        {
            await _context.StoredFiles.AddAsync(storedFile);

            await _context.SaveChangesAsync();

            return storedFile;
        }
        public async Task<StoredFile?> GetByIdAsync(Guid id)
        {
            return await _context.StoredFiles
                .FindAsync( id);
        }
    }
}