using ServiceBusBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusBot.Storage.Contract
{
    public  interface IStorageOrchastrator
    {
        ActionResponse ReadFileContentFromPath(string path);
    }
}
