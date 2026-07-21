using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
        :base ("Email or Password is invalid"){ 
        }
    }
}
