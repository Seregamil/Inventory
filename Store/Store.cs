using System;
using System.Collections.Generic;
using System.Linq;
using API.Database;
using API.Store.StorageData;
using GTANetworkAPI;
using Newtonsoft.Json;

namespace API.Store
{
    public class Store : IStore
    {
        private readonly Database.Profile _profile;
        private readonly Player _player;

        public Store(Player player, Database.Profile profile)
        {
            _player = player;
            _profile = profile;

            foreach (var storeItem in _profile.ProfileStores)
            {
                switch ((ItemType) storeItem.Type)
                {
                    case ItemType.VehicleKey:
                    case ItemType.HouseKey:
                        break;
                    case ItemType.Weapon:
                        storeItem.Object = JsonConvert.DeserializeObject<Weapon>(storeItem.ObjectData);
                        
                        var weapon = (Weapon) storeItem.Object;
                        player.GiveWeapon(NAPI.Util.WeaponNameToModel(weapon.Name), weapon.Ammo);
                        
                        Console.WriteLine($"Gived {weapon.Name} with {weapon.Ammo}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            _player.TriggerEvent("Storage:UpdateVehicleKeys", this.GetKeysOfVehicle());
        }

        public void Add(int slotId, StorageType storageType, string itemName, int itemType, int count, double weightPerOne,
            object data, int? entityType = null, int? entityId = null)
        {
            var item = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotId
                                     && x.StorageId == (int) storageType);

            if (item == null)
            { 
                // object obj = (ItemType) itemType switch
                // {
                //     ItemType.HouseKey => "",
                //     ItemType.VehicleKey => "",
                //     ItemType.Weapon => data as Weapon,
                //     _ => throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null)
                // };

                using var context = new EntryContext();
                var ps = new ProfileStore
                {
                    ProfileId = _profile.Id,
                    Name = itemName,
                    Object = data,
                    Count = count,
                    Weight = weightPerOne * count,
                    Type = itemType,
                    ObjectData = JsonConvert.SerializeObject(data),
                    EntityId = entityId,
                    EntityType = entityType,
                    StorageId = (int) storageType,
                    SlotId = slotId,
                };
                
                context.ProfileStores.Add(ps);
                context.SaveChanges();
                
                ps.Profile = _profile;

                _profile.ProfileStores.Add(ps);
            }
            else
            {
                item.Count += count;
                item.Save();
                /*switch ((ItemType) itemType)
                {
                    case ItemType.HouseKey:
                    case ItemType.VehicleKey:
                        item.Count += count;
                        break;
                    case ItemType.Weapon:
                        var weapon = (Weapon) item.Object;
                        var weaponData = (Weapon) data;
                        
                        weapon.Ammo += weaponData.Ammo;
                        
                        item.ObjectData = JsonConvert.SerializeObject(weapon);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
                }
                item.Save();*/
            }
            
            _player.TriggerEvent("Storage:UpdateVehicleKeys", this.GetKeysOfVehicle());
        }

        /// <summary>
        /// Очищает слот в инвентаре
        /// </summary>
        /// <param name="slotId">ID слота</param>
        /// <param name="storageType">Хранилище</param>
        /// <param name="count">Кол-во</param>
        /// <exception cref="Exception"></exception>
        public void Remove(int slotId, StorageType storageType, int count)
        {
            var item = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotId 
                                     && x.StorageId == (int) storageType);

            if (item == null)
                throw new Exception(
                    $"When removing item from store {_profile.Id} item is null {slotId}:{(storageType)}");

            if (item.Count - count > 0)
            {
                item.Count -= count;
                item.Save();
            }
            else
            {
                _profile.ProfileStores.Remove(item);

                using var context = new EntryContext();
                var itemInDb = context.ProfileStores
                    .FirstOrDefault(x => x.ProfileId == _profile.Id
                                         && x.SlotId == slotId 
                                         && x.StorageId == (int) storageType);

                if (itemInDb == null)
                    throw new Exception("[Store:Remove] -> Item in db doesnt exist");

                context.ProfileStores.Remove(itemInDb);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Перемещает предмет из слота в слот меж инвентарями или внутри них
        /// </summary>
        /// <param name="slotFromId"></param>
        /// <param name="slotToId"></param>
        /// <param name="storageFrom"></param>
        /// <param name="storageTo"></param>
        /// <exception cref="Exception">Срабатывает при отсуствии предмета в исходном слоте</exception>
        public void Move(int slotFromId, int slotToId, StorageType storageFrom, StorageType storageTo)
        {
            var sourceItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotFromId
                                     && x.StorageId == (int) storageFrom);

            if (sourceItem == null)
                throw new Exception("[pStore.Move] -> source item are empty!");
            
            var destinationItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotToId
                                     && x.StorageId == (int) storageTo);

            if (destinationItem != null)
            { // предмет в слоте назначения существует
                if(sourceItem.Name == destinationItem.Name // названия совпадают
                   && sourceItem.EntityId == destinationItem.EntityId // сущности совпадают
                   && sourceItem.EntityType == destinationItem.EntityType // Типы сущностей совпадают
                   && sourceItem.Type == destinationItem.Type) // типы предметов совпадают
                    this.Merge(slotFromId, slotToId, storageFrom, storageTo); // мержим предметы
                else
                    this.Swap(slotFromId, slotToId, storageFrom, storageTo); // меняем их местами
            }
            else
            {
                sourceItem.SlotId = slotToId;
                sourceItem.StorageId = (int) storageTo;
                
                sourceItem.Save();
            }
        }

        /// <summary>
        /// Разделяет предмет в инвентаре
        /// </summary>
        /// <param name="storageType">Инвентарь</param>
        /// <param name="slotSource">Исходный слот</param>
        /// <param name="count">Кол-во предметов, которое уберется</param>
        /// <param name="slotSet">Слот назначения</param>
        public void Split(StorageType storageType, int slotSource, int count, int slotSet)
        {
            var item = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotSource
                                     && x.StorageId == (int) storageType);

            if (item == null)
                throw new Exception("[pStore.Split] -> source item are empty!");
            
            // Кол-во предметов меньше запрашиваемового
            if(item.Count < count)
                return;
            
            // Кол-во станет равно 0
            if(item.Count - count == 0)
                return;

            item.Count -= count;
            item.Save();
            
            // Try to add item into storage
            this.Add(slotSet, storageType, item.Name, item.Type, count, item.Weight, item.ObjectData, item.EntityType, item.EntityId);
        }

        /// <summary>
        /// Проверяет наличие предмета у игрока в нужном хранилище
        /// </summary>
        /// <param name="itemName">Универсальное название предмета в базе данных</param>
        /// <param name="itemType">Тип предмета</param>
        /// <param name="storageType">Тип хранилища</param>
        /// <param name="entityType">Сущность, если есть</param>
        /// <param name="entityId">ID сущности, если есть</param>
        /// <returns></returns>
        public bool DoesHaveItem(string itemName, ItemType itemType, StorageType storageType, int entityType = -1, int entityId = -1)
        {
            var item = _profile.ProfileStores
                .FirstOrDefault(x => x.Name == itemName 
                                     && x.Type == (int) itemType 
                                     && x.StorageId == (int) storageType
                                     && x.EntityId == entityId
                                     && x.EntityType == entityType);

            if (item == null)
                return false;

            switch (itemType)
            {
                case ItemType.VehicleKey:
                case ItemType.HouseKey:
                    if (item.EntityType != entityType || item.EntityId != entityId)
                        return false;
                    break;
                default:
                    return false;
            }
            
            return true;
        }

        /// <summary>
        /// Совмещает предметы меж собой
        /// </summary>
        /// <param name="slotFrom">Исходный слот</param>
        /// <param name="slotTo">Слот назначения</param>
        /// <param name="storageTypeFrom">Исходное хранилище</param>
        /// <param name="storageTypeTo">Хранилище назначения</param>
        /// <exception cref="Exception"></exception>
        public void Merge(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo)
        {
            var sourceItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotFrom
                                     && x.StorageId == (int) storageTypeFrom);

            if (sourceItem == null)
                throw new Exception("[pStore.Merge] -> source item are empty!");
            
            var destinationItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotTo
                                     && x.StorageId == (int) storageTypeTo);
            
            if (destinationItem == null)
                throw new Exception("[pStore.Merge] -> destinationItem are empty!");

            destinationItem.Count += sourceItem.Count;
            destinationItem.Save();
            
            this.Remove(slotFrom, storageTypeFrom, sourceItem.Count);
        }

        /// <summary>
        /// Меняет местами элементы
        /// </summary>
        /// <param name="slotFrom">Исходный слот</param>
        /// <param name="slotTo">Слот назначения</param>
        /// <param name="storageTypeFrom">Исходное хранилище</param>
        /// <param name="storageTypeTo">Хранилище назначения</param>
        /// <exception cref="Exception"></exception>
        public void Swap(int slotFrom, int slotTo, StorageType storageTypeFrom, StorageType storageTypeTo)
        {
            var sourceItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotFrom
                                     && x.StorageId == (int) storageTypeFrom);

            if (sourceItem == null)
                throw new Exception("[pStore.Swap] -> source item are empty!");
            
            var destinationItem = _profile.ProfileStores
                .FirstOrDefault(x => x.SlotId == slotTo
                                     && x.StorageId == (int) storageTypeTo);
            
            if (destinationItem == null)
                throw new Exception("[pStore.Swap] -> destinationItem are empty!");

            sourceItem.SlotId = slotTo;
            sourceItem.StorageId = (int) storageTypeTo;
            sourceItem.Save();
            
            destinationItem.SlotId = slotTo;
            destinationItem.StorageId = (int) storageTypeTo;
            destinationItem.Save();        
        }
        
        private List<int?> GetKeysOfVehicle()
        {
            var keys = _profile.ProfileStores
                .Where(x => x.Type == (int) ItemType.VehicleKey 
                            && x.EntityId != null
                            && x.StorageId == (int) StorageType.Individual)
                
                .Select(x => x.EntityId)
                .ToList();
            
            keys.Add(-1);
            return keys;
        }

        public List<ProfileStore> GetItemsByType(int itemType)
        {
            return _profile.ProfileStores
                .Where(x => x.Type == itemType && x.StorageId == (int) StorageType.Individual)
                .ToList();
        }

        /// <summary>
        /// Возвращает первый свободный слот в инвентаре игрока
        /// </summary>
        /// <returns>-1 в случае отсуствия свободного слота</returns>
        public int GetFreeSlot()
        {
            for (var i = 1; i != 26; i++)
            {
                var slot = _profile.ProfileStores
                    .FirstOrDefault(x => x.StorageId == (int) StorageType.Individual 
                                         && x.SlotId == i);
                if (slot == null)
                    return i;
            }

            return -1;
        }
    }
}