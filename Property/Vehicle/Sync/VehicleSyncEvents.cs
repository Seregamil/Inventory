using System;
using GTANetworkAPI;

namespace GM.Property.Vehicle.Sync
{
    public class VehicleSyncEvents : Script
    {
        [ServerEvent(Event.VehicleTyreBurst)]
        public void OnVehicleTyreBurst(GTANetworkAPI.Vehicle source, int tyreIndex)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;
            
            source.SetWheelState(tyreIndex, WheelState.Burst);
            Console.WriteLine($"Vehicle {vehicle.Id} bursted wheel {tyreIndex}");
        }
        
        [ServerEvent(Event.VehicleDoorBreak)]
        public void OnVehicleDoorBreak(GTANetworkAPI.Vehicle source, int doorIndex)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;
            
            source.SetDoorState(doorIndex, DoorState.Broken);
            Console.WriteLine($"Vehicle {vehicle.Id} breaked door {doorIndex}");
        }
        
        [ServerEvent(Event.VehicleDamage)]
        public void OnVehicleDamageHandler(GTANetworkAPI.Vehicle source, float bodyHealthLoss, float engineHealthLoss)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;
            
            Console.WriteLine($"Vehicle {vehicle.Id} damaged. Engine: {engineHealthLoss} Body {bodyHealthLoss}");
        }
        
        [ServerEvent(Event.VehicleWindowSmash)]
        public void OnVehicleWindowSmash(GTANetworkAPI.Vehicle source, int windowIndex)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;
            
            Console.WriteLine($"Vehicle {vehicle.Id} window {windowIndex} smashed");
        }
    }
}