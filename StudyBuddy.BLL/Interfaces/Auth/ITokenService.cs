using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.BLL.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerateToken(User user);

    }
}
