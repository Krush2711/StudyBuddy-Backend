using System;
using System.Collections.Generic;
using System.Text;

// this for -> almost every table will have the common fileds , id , created at , apdated at etc...


namespace StudyBuddy.Models.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
