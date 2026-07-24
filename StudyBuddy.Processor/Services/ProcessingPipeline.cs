using StudyBuddy.Processor.Interfaces;
using StudyBuddy.Processor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Services
{
    public class ProcessingPipeline : IProcessingPipeline
    {
        private readonly IProcessorFactory _factory;

        public ProcessingPipeline(IProcessorFactory factory)
        {
                _factory = factory;
        }

        public async Task <ProcessingResult> ProcessAsync(ProcessingContext context)

        {
            var processor = _factory.GetProcesser(context.FileExtension); 
            
            return await processor.ProcessAsync(context);
        }
    }
}
