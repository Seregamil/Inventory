using System;
using API.Store;
using GM.Client;
using GTANetworkAPI;
using Newtonsoft.Json;
using EntityType = API.Store.EntityType;

namespace GM.Property.Vehicle
{
    public static class Util
    {
        public static API.Database.Vehicle GetInfo(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasData("data") ? null : vehicle.GetData<API.Database.Vehicle>("data");
        }

        public static void SetInfo(this GTANetworkAPI.Vehicle vehicle, API.Database.Vehicle info)
        {
            vehicle.SetData("data", info);
        }

        public static void SetPrimaryColor(this GTANetworkAPI.Vehicle vehicle, int clr)
        {
            vehicle.SetSharedData("V_CLR_1", clr);
            vehicle.PrimaryColor = clr;
        }
        
        public static void SetSecondaryColor(this GTANetworkAPI.Vehicle vehicle, int clr)
        {
            vehicle.SetSharedData("V_CLR_2", clr);
            vehicle.SecondaryColor = clr;
        }
        
        public static bool DoesHaveKey(this Player player, GTANetworkAPI.Vehicle source)
        {
            var profile = player.GetProfile();
            var vehicle = source.GetInfo();
            if (profile == null || vehicle == null)
                return false;

            if (vehicle.Id == -1)
                return true;
            
            return profile.Store.DoesHaveItem(StorageItemNames.VehicleKey, 
                ItemType.VehicleKey, 
                StorageType.Individual,
                (int) EntityType.Vehicle,
                vehicle.Id);
        }

        public static void Save(this GTANetworkAPI.Vehicle source)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;

            vehicle.Color1 = source.GetSharedData<int>("V_CLR_1");
            vehicle.Color2 = source.GetSharedData<int>("V_CLR_2");

            vehicle.X = source.Position.X;
            vehicle.Y = source.Position.Y;
            vehicle.Z = source.Position.Z;
            
            vehicle.Dimension = source.Dimension;
            
            vehicle.BonesData = JsonConvert.SerializeObject(vehicle.VehicleBones);
            vehicle.Save();
        }
    }
}