using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Service1
{
    public interface IService1 : IService
    {
        Task<bool> TestMethod();
    }
}
