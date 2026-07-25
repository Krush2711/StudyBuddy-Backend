using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Processors
{
    public class SourceCodeProcessor : IProcesser
    {
        private static readonly HashSet<string> SupportedExtensions = [".cs",
        ".vb",
        ".cpp",
        ".c",
        ".h",
        ".hpp",
        ".java",
        ".py",
        ".js",
        ".ts",
        ".tsx",
        ".jsx",
        ".go",
        ".rs",
        ".kt",
        ".swift",
        ".php",
        ".rb",
        ".scala",
        ".html",
        ".css",
        ".scss",
        ".sql",
        ".json",
        ".xml",
        ".yml",
        ".yaml",
        ".toml",
        ".ini",
        ".config",
        ".env",
        ".md",
        ".dockerfile"];

        public bool CanProcess(string extension)
        {
            return SupportedExtensions.Contains(extension.ToLowerInvariant());
        }

        public async Task<ProcessingResult> ProcessAsync(ProcessingContext context)
        {
            var result = new ProcessingResult();
            try
            {
                var source = await File.ReadAllTextAsync(context.FilePath);
                result.Success = true;
                result.ExtractedText = source;
                result.CharacterCount = source.Length;
                result.PageCount = 1; // Source code files are typically considered as single-page documents

                result.Metadata["Processor"] = "SourceCodeProcessor";
                result.Metadata["FileExtension"] = context.FileExtension;
                //result.Metadata["Language"] = GetLanguage(context.FileExtension);
                result.Metadata["Filename"] = Path.GetFileName(context.FilePath);

                return result;

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ExtractedText = ex.Message;
                return result;
            }
        }


        private static string GetLanguage(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".cs" => "C#",
                ".vb" => "VB.NET",
                ".cpp" => "C++",
                ".c" => "C",
                ".h" => "C Header",
                ".hpp" => "C++ Header",
                ".py" => "Python",
                ".java" => "Java",
                ".js" => "JavaScript",
                ".ts" => "TypeScript",
                ".tsx" => "TypeScript React",
                ".jsx" => "JavaScript React",
                ".go" => "Go",
                ".rs" => "Rust",
                ".kt" => "Kotlin",
                ".swift" => "Swift",
                ".php" => "PHP",
                ".rb" => "Ruby",
                ".scala" => "Scala",
                ".html" => "HTML",
                ".css" => "CSS",
                ".scss" => "SCSS",
                ".sql" => "SQL",
                ".json" => "JSON",
                ".xml" => "XML",
                ".yml" => "YAML",
                ".yaml" => "YAML",
                ".toml" => "TOML",
                ".ini" => "INI",
                ".config" => "Configuration",
                ".env" => "Environment Variables",
                ".md" => "Markdown",
                ".dockerfile" => "Dockerfile",
                _ => "Unknown"
            };

        }
    }
}
