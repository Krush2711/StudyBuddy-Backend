using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Knowledge.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string KnowledgeCollectionName { get; set; } = "KnowledgeDocuments";
    }
}
