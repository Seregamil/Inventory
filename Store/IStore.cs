namespace API.Store
{
    public interface IStore
    {
        void Add(int slotId, StorageType storageType, string itemName, int itemType, int count, double weightPerOne, object data, int? entityType = null,
            int? entityId = null);

        void Remove(int slotId, StorageType storageType, int count);
        void Move(int slotFromId, int slotToId, StorageType storageFrom, StorageType storageTo);
        void Split(StorageType storageType, int slotSource, int count, int slotSet);
        bool DoesHaveItem(string itemName, ItemType itemType, StorageType storageType, int entityType = -1, int entityId = -1);

        void Merge(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo);
        void Swap(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo);
        int GetFreeSlot();
    }
}