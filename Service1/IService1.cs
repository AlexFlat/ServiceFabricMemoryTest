using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Service1.Data;

namespace Service1
{
    public interface IService1 : IService
    {
        Task<bool> TestMethod();

        Task Add(Item item);
        Task Delete(int count);

        Task<long> GetCount();
        Task<long> GetCountTraverse();


    }
}
