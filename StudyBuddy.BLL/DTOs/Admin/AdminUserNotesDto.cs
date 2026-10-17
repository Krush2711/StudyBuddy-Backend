using StudyBuddy.Knowledge.Models;

namespace StudyBuddy.BLL.DTOs.Admin
{
    public class AdminUserNotesDto
    {
        public Guid UserId { get; set; }
        public List<DetailedNotesDocument> DetailedNotes { get; set; } = new();
        public List<ShortNotesDocument> ShortNotes { get; set; } = new();
    }
}
