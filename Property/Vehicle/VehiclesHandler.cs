using System;
using System.Linq;
using API.Database;
using API.Partial;
using API.Store;
using GM.Property.Vehicle.Sync;
using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GM.Property.Vehicle
{
    public class VehiclesHandler : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            using var context = new EntryContext();
            context.Vehicles
                .Include(s => s.VehicleStorages)
                .ToList()
                .ForEach(x => CreateVehicle(x));
        }

        public static GTANetworkAPI.Vehicle CreateVehicle(API.Database.Vehicle vehicle)
        {
            var model = NAPI.Util.GetHashKey(vehicle.ModelName);
            
            var source = NAPI.Vehicle.CreateVehicle(model, new Vector3(vehicle.X, vehicle.Y, vehicle.Z), vehicle.Heading,
                vehicle.Color1, vehicle.Color2, vehicle.NumberPlate, 255, vehicle.IsLocked, vehicle.Engine, (uint) vehicle.Dimension);
            
            vehicle.VehicleBones = JsonConvert.DeserializeObject<VehicleBones>(vehicle.BonesData);
            vehicle.Store = new VehicleStore(source, vehicle);
            
            source.SetInfo(vehicle);
            
            source.SetSharedData("ID", vehicle.Id);
            
            source.SetSharedData("V_CLR_1", vehicle.Color1);
            source.SetSharedData("V_CLR_2", vehicle.Color2);
            
            source.SetSharedData("V_LOCKED", vehicle.IsLocked);
            source.SetSharedData("V_ENGINE", vehicle.Engine);
            
            source.SetSharedData("V_FUEL", Convert.ToInt32(vehicle.Fuel));
            source.SetSharedData("MILEAGE", (int)vehicle.Miliage);
            source.SetSharedData("MAX_FUEL", vehicle.MaxFuel);
            source.SetSharedData("OWNER_ID", vehicle.OwnerId ?? -1);
            
            source.SetSharedData("V_DIRT", vehicle.Dirt);
                
            source.SetSharedData("V_BODY", vehicle.BodyHealth);
            source.SetSharedData("V_ENGINEHEALTH", vehicle.EngineHealth);
            source.SetSharedData("V_HANDBRAKE", false);
            
            source.SetSharedData("V_LEFT_LED", false);
            source.SetSharedData("V_RIGHT_LED", false);

            for (var i = 0; i != vehicle.VehicleBones.Doors.Count; i++)
                source.SetSharedData($"VSync:{VehicleBoneNames.DoorNames[i]}", vehicle.VehicleBones.Doors[i]);
                    
            for (var i = 0; i != vehicle.VehicleBones.Wheels.Count; i++)
                source.SetSharedData($"VSync:{VehicleBoneNames.WheelNames[i]}", vehicle.VehicleBones.Wheels[i]);
            
            return source;
        }

        public static void RemoveVehicle(GTANetworkAPI.Vehicle source)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;

            var id = vehicle.Id;

            using var context = new EntryContext();
            var vehInDb = context.Vehicles.FirstOrDefault(x => x.Id == id);
            if(vehInDb == null)
                return;
            
            context.Remove(vehInDb);
            context.SaveChanges();
            
            source.Delete();
        }
    }
}