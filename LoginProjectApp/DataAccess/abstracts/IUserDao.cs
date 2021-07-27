using LoginProjectApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginProjectApp.DataAccess.abstracts
{
    public interface IUserDao
    {
        void AddUser(string email, string password);
        void AddToken(User user);
        Task<bool> CheckEmailAndPassword(string email, string password);
        

    }
}
