using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StudyBuddy.Knowledge.Configuration;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.Knowledge.Repositories;

public class ShortNotesRepository : IShortNotesRepository
{
    private readonly IMongoCollection<ShortNotesDocument> _collection;

    public ShortNotesRepository(
        IMongoClient client,
        IOptions<MongoDbSettings> settings)
    {
        var database = client.GetDatabase(settings.Value.DatabaseName);

        _collection = database.GetCollection<ShortNotesDocument>(
            settings.Value.ShortNotesCollectionName);
    }

    public async Task CreateAsync(ShortNotesDocument document)
    {
        await _collection.InsertOneAsync(document);
    }

    public async Task<ShortNotesDocument?> GetByIdAsync(Guid id)
    {
        return await _collection
            .Find(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ShortNotesDocument>> GetByUserAsync(
        Guid userId,
        DateTime? fromUtc = null,
        string? subjectOrTag = null)
    {
        var filters = new List<FilterDefinition<ShortNotesDocument>>
        {
            Builders<ShortNotesDocument>.Filter.Eq(x => x.UserId, userId)
        };

        if (fromUtc.HasValue)
        {
            filters.Add(Builders<ShortNotesDocument>.Filter
                .Gte(x => x.CreatedAt, fromUtc.Value));
        }

        if (!string.IsNullOrWhiteSpace(subjectOrTag))
        {
            filters.Add(Builders<ShortNotesDocument>.Filter
                .Eq(x => x.Tag, subjectOrTag));
        }

        return await _collection
            .Find(Builders<ShortNotesDocument>.Filter.And(filters))
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

        public async Task DeleteByUserAsync(Guid userId)
        {
            await _collection.DeleteManyAsync(x => x.UserId == userId);
        }

}