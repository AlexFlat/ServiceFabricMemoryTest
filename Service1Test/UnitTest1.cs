using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service1;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.Threading.Tasks;
using Service1.Data;

namespace Service1Test
{
    [TestClass]
    public class UnitTest1
    {
        private const string SERVICEURL = "fabric:/ServiceFabricTest/Service1";
        private const int ITEM_COUNT = 10000;

        [TestMethod]
        public void TestMethod1()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            Parallel.For(0, 100000, (current) =>
                {
                    var result = proxy.TestMethod().Result;
                }
            );

        }

        [TestMethod]
        public void AddItems()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            var item = new Item();
            item.MetaDatas = new MetaDataList();
            for (int i = 0; i < 100; i++)
            {
                item.MetaDatas.AddOrUpdate(i.ToString(), $"Value {i}");
            }
            Parallel.For(0, ITEM_COUNT, (current) =>
            {
                proxy.Add(item).Wait();
            }
            );

        }

        [TestMethod]
        public void DeleteItems()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            proxy.Delete(ITEM_COUNT).Wait();
        }

        [TestMethod]
        public void DictionaryCount()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            var result = proxy.GetCount().Result;
            Assert.IsTrue(false, $"Count Equals {result}");
        }

        [TestMethod]
        public void DictionaryCountTraverse()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            var result = proxy.GetCountTraverse().Result;
            Assert.IsTrue(false, $"Count Equals {result}");
        }

        [TestMethod]
        public void SetDictionaryModeServiceFabric()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            proxy.UseServiceFabricState(true).Wait();
        }

        [TestMethod]
        public void SetDictionaryModeMemory()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            proxy.UseServiceFabricState(false).Wait();
        }

        [TestMethod]
        public void GCCollect()
        {
            var url = new Uri(SERVICEURL);
            var proxy = ServiceProxy.Create<IService1>(url, new Microsoft.ServiceFabric.Services.Client.ServicePartitionKey(0));
            proxy.GCCollect().Wait();
        }

    }
}
