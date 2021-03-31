using System;
using System.Collections.Generic;
using System.Linq;
using API.Store;
using GM.Client;
using GM.Global;
using GM.Property.Vehicle;
using GTANetworkAPI;
using Newtonsoft.Json;
using EntityType = API.Store.EntityType;

namespace GM.Property.Storage
{
    public class Commands : Script
    {
        [Command("keyadd")]
        public void keyAdd_CMD(Player player, int type, int entityId)
        {
            var profile = player.GetProfile();
            if(profile == null)
                return;

            var slot = profile.Store.GetFreeSlot();
            if (slot == -1)
            {
                player.ShowNotify(Notify.MessageType.Alert, "Ошибка", $"Инвентарь полон");
                return;
            }
            
            var itemName = (ItemType) type switch
            {
                ItemType.HouseKey => StorageItemNames.HouseKey,
                ItemType.VehicleKey => StorageItemNames.VehicleKey,
                _ => null
            };
            
            var entityType = (ItemType) type switch
            {
                ItemType.HouseKey => (int?) EntityType.House,
                ItemType.VehicleKey => (int?) EntityType.Vehicle,
                _ => null
            };

            profile.Store.Add(slot, StorageType.Individual, itemName, type, 1, 0.5, new object(), entityType, entityId);
            
            player.ShowNotify(Notify.MessageType.Success, "Ключ добавлен", $"{itemName} from {entityId} was added into store");
        }

        [Command("keylist")]
        public void keyList_CMD(Player player)
        {
            var profile = player.GetProfile();
            if(profile == null)
                return;

            var keys = profile.Store.GetItemsByType((int) ItemType.VehicleKey);
            keys.AddRange(profile.Store.GetItemsByType((int) ItemType.HouseKey));

            if (keys.Count == 0)
            {
                player.ShowNotify(Notify.MessageType.Alert, "No one keys", "У вас отсутствуют какие-либо ключи");
                return;
            }

            var arr = new List<string>();
            keys.ForEach(x =>
            { 
                var name = "";
                switch ((ItemType) x.Type)
                {
                    case ItemType.HouseKey:
                        name = "Unknown";
                        break;
                    case ItemType.VehicleKey:
                        var info = NAPI.Pools.GetAllVehicles()
                            .Select(z => new {data = z.GetInfo(), z.DisplayName})
                            .Where(z => z.data != null && z.data.Id != -1)
                            .FirstOrDefault(z => z.data.Id == x.EntityId);
                        
                        if(info == null)
                            return;

                        name = info.DisplayName;
                        break;
                    default:
                        name = "";
                        break;
                }

                arr.Add($"ID: {x.EntityId} {name} Кол-во: {x.Count}");
            });
            
            player.ShowModal("Ключи", JsonConvert.SerializeObject(arr));
        }
    }
}