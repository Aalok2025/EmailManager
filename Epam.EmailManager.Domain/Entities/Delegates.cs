using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epam.EmailManager.Domain.Entities
{
    public delegate void EmailSentHandler(string recipient, string subject, string body);
}
