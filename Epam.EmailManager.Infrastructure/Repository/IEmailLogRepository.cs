using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epam.EmailManager.Infrastructure.Repository
{
    public interface IEmailLogRepository<T>
    {
        void LogEmailDetails(T recipient, T subject, T body);
    }
}
