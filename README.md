# ServiceFabricMemoryTest
Reproduces memory issues found in ReliableCollections

# Process
- Build and Deploy the project

# Test - ConcurrentDictionary
- Run Test "SetDictionaryModeMemory()"
- Run Test "AddItems()" - this adds 10k items to a ConcurrentDictionary within the service
- View Memory Usage - should be approx 200-300mb
- Run Test "DeleteItems()" - this will remove 10k items
- View Memory Usage - should still be approx 200-300mb
- Run Test "GCCollect()" - this runs GCCollect within Service1
- View Memory Usage - should drop a little
- Run Test "GCCollect()" again
- View Memory Usage - should drop to approx 50mb again

# Test - ServiceFabric ReliableDictionary
- Run Test "SetDictionaryModeServiceFabric()"
- Run Test "AddItems()" - this adds 10k items to a ReliableDictionary within the service
- View Memory Usage - should be approx 200-300mb
- Run Test "DeleteItems()" - this will remove 10k items
- View Memory Usage - should still be approx 200-300mb
- Run Test "GCCollect()" - this runs GCCollect within Service1
- View Memory Usage - should drop a little
- Run Test "GCCollect()" again
- View Memory Usage - never drops any further




