using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId)
           : base($"User with Id '{userId}' was not found.")
        {
        }
    }
}
