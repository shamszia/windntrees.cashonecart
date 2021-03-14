using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IEmailer
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
