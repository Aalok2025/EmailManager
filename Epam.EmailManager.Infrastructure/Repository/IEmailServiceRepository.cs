using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epam.EmailManager.Domain.Entities;

namespace Epam.EmailManager.Infrastructure.Repository
{
    public interface IEmailServiceRepository<T>
    {
        public Task<bool> sendEmailToAllUsers(List<User> users);
        public Task<bool> sendEmailToUserWithId(T id);
    }
}
