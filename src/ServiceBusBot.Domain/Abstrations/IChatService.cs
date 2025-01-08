using ServiceBusBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusBot.Domain.Abstrations
{
    public interface IChatService
    {
        Task<ModelResponse?> GetResponseAsync(string message);
    }
}
