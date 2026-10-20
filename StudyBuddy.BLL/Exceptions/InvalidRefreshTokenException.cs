using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Exceptions
{
    /// <summary>
    /// Raised when a refresh token is missing, unknown, expired or revoked.
    /// Mapped to 401 so the client clears its session and re-authenticates.
    /// </summary>
    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException()
        : base ("The session could not be renewed. Please sign in again."){
        }
    }
}
