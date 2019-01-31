using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Data;
using System.Diagnostics;
using Service1.Data;
using Serilog;
using Serilog.Sinks.File;
using System.Collections.Concurrent;

namespace Service1
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class Service1 : StatefulService, IService1
    {
        private bool _useServiceFabricState = true;
        private ConcurrentDictionary<string, Item> _concurrentDictionary = new ConcurrentDictionary<string, Item>();

        public Service1(StatefulServiceContext context)
            : base(context)
        {
            var log = new LoggerConfiguration();
            log.WriteTo.Async(a => a.File(context.CodePackageActivationContext.LogDirectory + "\\Service1.log",
            fileSizeLimitBytes: 100 * 1024,
            shared: true)
            );
            Log.Logger = log.CreateLogger();
        }

        public async Task Add(Item item)
        {
            if (_useServiceFabricState)
            {
                using (var tx = StateManager.CreateTransaction())
                {

                    await (await GetDictionary()).AddAsync(tx, Guid.NewGuid().ToString(), item);
                    await tx.CommitAsync();
                }
            }
            else
            {
                _concurrentDictionary.TryAdd(Guid.NewGuid().ToString(), item);
            }
        }

        public Task<bool> TestMethod()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context =>
                        this.CreateServiceRemotingListener(context))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            return Task.FromResult(true);
        }

        private async Task<IReliableDictionary<string, Item>> GetDictionary()
        {
            return await StateManager.GetOrAddAsync<IReliableDictionary<string, Item>>("Dictionary");
        }

        public async Task<long> GetCount()
        {
            if(_useServiceFabricState)
            {
                var dict = await GetDictionary();
                var ct = new CancellationToken();
                using (var tx = StateManager.CreateTransaction())
                {
                    return await dict.GetCountAsync(tx);
                }
            }
            else
            {
                return _concurrentDictionary.Count;
            }
        }

        public async Task<long> GetCountTraverse()
        {
            long currentCount = 0;
            if (_useServiceFabricState)
            {
                var dict = await GetDictionary();
                var ct = new CancellationToken();
                using (var tx = StateManager.CreateTransaction())
                {
                    var enumerable = await dict.CreateEnumerableAsync(tx);
                    using (var enumerator = enumerable.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(ct))
                        {
                            var current = enumerator.Current;
                            currentCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < _concurrentDictionary.Keys.Count; i++)
                {
                    var current = _concurrentDictionary.ElementAt(i);
                    currentCount++;
                }
            }
            return currentCount;
        }

        public async Task<long> GetCountTraverseGetKey()
        {
            long currentCount = 0;
            if (_useServiceFabricState)
            {
                var dict = await GetDictionary();
                var ct = new CancellationToken();
                using (var tx = StateManager.CreateTransaction())
                {
                    var enumerable = await dict.CreateEnumerableAsync(tx);
                    using (var enumerator = enumerable.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(ct))
                        {
                            var current = enumerator.Current;
                            var exists = await dict.ContainsKeyAsync(tx, current.Key);
                            currentCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < _concurrentDictionary.Keys.Count; i++)
                {
                    var current = _concurrentDictionary.ElementAt(i);
                    currentCount++;
                }
            }
            return currentCount;
        }
        public async Task<long> GetCountTraverseGetValue()
        {
            long currentCount = 0;
            if (_useServiceFabricState)
            {
                var dict = await GetDictionary();
                var ct = new CancellationToken();
                using (var tx = StateManager.CreateTransaction())
                {
                    var enumerable = await dict.CreateEnumerableAsync(tx);
                    using (var enumerator = enumerable.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(ct))
                        {
                            var current = enumerator.Current;
                            var copy = await dict.TryGetValueAsync(tx, current.Key);
                            currentCount++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < _concurrentDictionary.Keys.Count; i++)
                {
                    var current = _concurrentDictionary.ElementAt(i);
                    currentCount++;
                }
            }
            return currentCount;
        }

        private async Task DeleteItem( string key)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var dict = await GetDictionary();
                await dict.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }

        public async Task Delete(int count)
        {
            if(_useServiceFabricState)
            {
                var dict = await GetDictionary();
                var currentCount = 0;
                var ct = new CancellationToken();
                using (var tx = StateManager.CreateTransaction())
                {
                    var enumerable = await dict.CreateEnumerableAsync(tx);
                    using (var enumerator = enumerable.GetAsyncEnumerator())
                    {
                        while (await enumerator.MoveNextAsync(ct))
                        {
                            if (currentCount >= count)
                            {
                                Log.Information($"{nameof(RunAsync)} - Count {currentCount} - Exited");
                                break;
                            }
                            var current = enumerator.Current;
                            await DeleteItem(current.Key);
                            currentCount++;
                        }
                    }
                }
            }
            else
            {
                var deleted = 0;
                while (_concurrentDictionary.Count > 0)
                {
                    _concurrentDictionary.TryRemove(_concurrentDictionary.Keys.ElementAt(0), out var item);
                    deleted++;
                    if (deleted >= count)
                    {
                        break;
                    }
                }
            }
        }

        public Task UseServiceFabricState(bool enable)
        {
            _useServiceFabricState = enable;
            return Task.FromResult(true);
        }

        public Task GCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return Task.FromResult(true);
        }
    }
}
