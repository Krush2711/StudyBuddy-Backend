using StudyBuddy.Processor.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace StudyBuddy.Processor.Factory
{
    public class ProcessorFactory : IProcessorFactory
    {
        private readonly IEnumerable<IProcesser> _processers;
        
        public ProcessorFactory(IEnumerable<IProcesser> processers)
        {
            _processers = processers;
        }

        public IProcesser GetProcesser(string extension)
        {
            var processer = _processers.FirstOrDefault( x => x.CanProcess(extension));
            if (processer == null)
            {
                throw new NotSupportedException($"No processor found for {extension}");
            }
            return processer;
        }
    }
}
