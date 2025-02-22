using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epam.EmailManager.Domain.Entities;

namespace Epam.EmailManager.Infrastructure.Repository
{
    public interface IUserDetailsRepository<T> where T : User
    {
        public List<T> GetAllUsers();
        public int AddUser(T user);

        public User GetUserById(int id);
    }
}
