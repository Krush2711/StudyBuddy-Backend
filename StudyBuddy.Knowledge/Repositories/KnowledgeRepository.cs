using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Driver;
using StudyBuddy.Knowledge.Configuration;
using StudyBuddy.Knowledge.Interfaces;
using StudyBuddy.Knowledge.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.Extensions.Options;

namespace StudyBuddy.Knowledge.Repositories
{
    public class KnowledgeRepository : IKnowledgeRepository
    {
        private readonly IMongoCollection<KnowledgeDocument> _collection;
        
        public KnowledgeRepository(IMongoClient client , IOptions<MongoDbSettings> settings )
        {
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<KnowledgeDocument>(settings.Value.KnowledgeCollectionName);
        }

        public async Task<KnowledgeDocument> CreateAsync(KnowledgeDocument document)
        {
            await _collection.InsertOneAsync(document);

            return document;
        }

        public async Task<KnowledgeDocument?> GetByIdAsync(Guid id)
        {
            return await _collection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<KnowledgeDocument>> GetByUserAsync(Guid userId)
        {
            return await _collection
                .Find(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task UpdateSummaryAsync(Guid rawDocumentId,string summary)
        {
            var update = Builders<KnowledgeDocument>.Update.Set(x => x.Summary, summary);

            await _collection.UpdateOneAsync(
                x => x.RawDocumentId == rawDocumentId,
                update);
        }
    }

}

    //Things You'll Learn Here

    //This phase introduces several MongoDB concepts:

    //1.IMongoClient

    //Equivalent to your SQL connection. Register it as a Singleton because it's thread-safe and intended to be reused.

    //2. Database
    //client.GetDatabase("StudyBuddy")

    //Similar to selecting a database in SQL Server.

    //3. Collection
    //database.GetCollection<KnowledgeDocument>("KnowledgeDocuments")

    //Equivalent to a SQL table.

    //4. Find()

    //Unlike EF Core's LINQ over DbSet, MongoDB starts with Find():

    //.Find(x => x.UserId == userId)

    //This translates to a MongoDB filter.

    //5. InsertOneAsync()

    //Equivalent to:

    //_context.Users.Add(user);

    //await _context.SaveChangesAsync();

    //In MongoDB:

    //await _collection.InsertOneAsync(document);

    //No separate SaveChanges() is required.