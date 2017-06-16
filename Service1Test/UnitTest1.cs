using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service1;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Threading.Tasks;

namespace Service1Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var url = new Uri("fabric:/ServiceFabricTest/Service1");
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            Parallel.For(0, 100000, (current) =>
                {
                    var result = proxy.TestMethod().Result;
                }
            );

        }
    }
}
