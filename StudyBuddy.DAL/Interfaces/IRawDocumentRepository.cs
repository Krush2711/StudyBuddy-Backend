using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.DAL.Interfaces
{
    public interface IRawDocumentRepository
    {
        Task<RawDocument> AddAsync(RawDocument document);

        Task<RawDocument?> GetByIdAsync(Guid id);

        Task UpdateAsync(RawDocument rawDocument);

    }
}
