using StudyBuddy.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyBuddy.DAL.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User> CreateUSerAsync(User user);
    }
}
