using StudyBuddy.Knowledge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Interfaces
{
    public interface IDetailedNotesRepository
    {
        Task CreateAsync(DetailedNotesDocument document);

        Task<DetailedNotesDocument?> GetByRawDocumentIdAsync(Guid rawDocumentId);

        Task DeleteAsync(Guid id);
    }
}
