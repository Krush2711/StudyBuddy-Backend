using StudyBuddy.DAL.Context;
using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using StudyBuddy.DAL.Interfaces;

namespace StudyBuddy.DAL.Repositories
{
    public class RawDocumentRepository : IRawDocumentRepository
    {
        private readonly AppDbContext _context;

        public RawDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RawDocument> AddAsync(RawDocument document)
        {
            await _context.RawDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<RawDocument?> GetByIdAsync(Guid id)
        {
            return await _context.RawDocuments.FindAsync(id);
        }
    }
}
