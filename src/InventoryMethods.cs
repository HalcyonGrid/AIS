using OpenMetaverse;
using OpenSim.Data;
using InWorldz.Data.Inventory.Cassandra;

namespace AIS
{
    class InventoryMethods
    {
        private UUID _Id;

        private IInventoryStorage _storage;

        private InventoryStorage _cassandraStorage;
        private CassandraMigrationProviderSelector _selector;
    }
}
