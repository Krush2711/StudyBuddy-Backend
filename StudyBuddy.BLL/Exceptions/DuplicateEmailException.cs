using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string email) : base(
            $"Email '{email}' is already registered.")
        {

        }
    }
}
