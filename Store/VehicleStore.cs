using GTANetworkAPI;

namespace API.Store
{
    public class VehicleStore : IStore
    {
        private readonly Database.Vehicle _vehicle;
        private readonly Vehicle _source;
        
        public VehicleStore(Vehicle source, Database.Vehicle vehicle)
        {
            _vehicle = vehicle;
            _source = source;
        }


        public void Add(int slotId, StorageType storageType, string itemName, int itemType, int count, double weightPerOne,
            object data, int? entityType = null, int? entityId = null)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(int slotId, StorageType storageType, int count)
        {
            throw new System.NotImplementedException();
        }

        public void Move(int slotFromId, int slotToId, StorageType storageFrom, StorageType storageTo)
        {
            throw new System.NotImplementedException();
        }

        public void Split(StorageType storageType, int slotSource, int count, int slotSet)
        {
            throw new System.NotImplementedException();
        }

        public bool DoesHaveItem(string itemName, ItemType itemType, StorageType storageType, int entityType = -1, int entityId = -1)
        {
            throw new System.NotImplementedException();
        }

        public void Merge(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo)
        {
            throw new System.NotImplementedException();
        }

        public void Swap(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo)
        {
            throw new System.NotImplementedException();
        }

        public int GetFreeSlot()
        {
            throw new System.NotImplementedException();
        }
    }
}