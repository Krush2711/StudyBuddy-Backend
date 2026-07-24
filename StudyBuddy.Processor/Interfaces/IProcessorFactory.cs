using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.Processor.Interfaces
{
    public interface IProcessorFactory
    {
        IProcesser GetProcesser(string extension);
    }
}
