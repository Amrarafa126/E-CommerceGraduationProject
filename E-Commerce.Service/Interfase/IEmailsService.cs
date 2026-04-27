using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Service.Interfase
{
    public interface IEmailsService
    {
        public Task<string> SendEmail(string email, string Message, string? reason);

    }
}
