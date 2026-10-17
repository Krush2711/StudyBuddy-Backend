using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudyBuddy.Knowledge.Configuration;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Repositories
{
    public class DetailedNotesRepository : IDetailedNotesRepository
    {
        private readonly IMongoCollection<DetailedNotesDocument> _collection;

        public DetailedNotesRepository(
            IMongoClient client,
            IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _collection = database.GetCollection<DetailedNotesDocument>(
                "DetailedNotes");
        }

        public async Task CreateAsync(DetailedNotesDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task<DetailedNotesDocument?> GetByRawDocumentIdAsync(Guid rawDocumentId)
        {
            return await _collection
                .Find(x => x.RawDocumentId == rawDocumentId)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<DetailedNotesDocument>> GetByUserAsync(Guid userId)
        {
            return await _collection
                .Find(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteByUserAsync(Guid userId)
        {
            await _collection.DeleteManyAsync(x => x.UserId == userId);
        }

    }
}
