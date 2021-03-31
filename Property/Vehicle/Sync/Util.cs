using System;

namespace GM.Property.Vehicle.Sync
{
    public static class Util
    {
        public static DoorState GetDoorState(this GTANetworkAPI.Vehicle vehicle, int doorId)
        {
            var sharedName = $"VSync:{VehicleBoneNames.DoorNames[doorId]}";
            if (!vehicle.HasSharedData(sharedName))
                return (int) DoorState.Closed;

            return (DoorState) vehicle.GetSharedData<int>(sharedName);
        }
        
        public static WheelState GetWheelState(this GTANetworkAPI.Vehicle vehicle, int wheelId)
        {
            var sharedName = $"VSync:{VehicleBoneNames.WheelNames[wheelId]}";
            if (!vehicle.HasSharedData(sharedName))
                return (int) WheelState.Fixed;

            return (WheelState) vehicle.GetSharedData<int>(sharedName);
        }
        
        public static void SetDoorState(this GTANetworkAPI.Vehicle vehicle, int doorId, DoorState state)
        {
            var sharedName = $"VSync:{VehicleBoneNames.DoorNames[doorId]}";
            if (!vehicle.HasSharedData(sharedName))
                return;

            vehicle.SetSharedData(sharedName, (int) state);
        }
        
        public static void SetWheelState(this GTANetworkAPI.Vehicle vehicle, int doorId, WheelState state)
        {
            var sharedName = $"VSync:{VehicleBoneNames.WheelNames[doorId]}";
            if (!vehicle.HasSharedData(sharedName))
                return;
            
            vehicle.SetSharedData(sharedName, (int) state);
        }

        public static bool GetLockState(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasSharedData("V_LOCKED") || vehicle.GetSharedData<bool>("V_LOCKED");
        }

        public static void SetLockState(this GTANetworkAPI.Vehicle source, bool state)
        {
            var vehicle = source.GetInfo();

            if(!source.HasSharedData("V_LOCKED") || vehicle == null)
                return;

            vehicle.IsLocked = state;
            source.SetSharedData("V_LOCKED", state);
        }

        public static bool GetEngineState(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasSharedData("V_ENGINE") || vehicle.GetSharedData<bool>("V_ENGINE");
        }
        
        public static void SetEngineState(this GTANetworkAPI.Vehicle source, bool state)
        {
            var vehicle = source.GetInfo();
            
            if(!source.HasSharedData("V_ENGINE") || vehicle == null)
                throw new Exception($"SetEngineState -> source info or shared does null");

            source.SetSharedData("V_ENGINE", state);
            vehicle.Engine = state;
            
            Console.WriteLine($"[V-ID #{vehicle.Id}] Engine state is {state}");
        }

        public static bool GetHandbrakeState(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasSharedData("V_HANDBRAKE") || vehicle.GetSharedData<bool>("V_HANDBRAKE");
        }
        
        public static void SetHandbrakeState(this GTANetworkAPI.Vehicle vehicle, bool state)
        {
            if(!vehicle.HasSharedData("V_HANDBRAKE"))
                return;

            vehicle.SetSharedData("V_HANDBRAKE", state);
        }
        
        public static bool GetRightIndicator(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasSharedData("V_RIGHT_LED") || vehicle.GetSharedData<bool>("V_RIGHT_LED");
        }
        
        public static void SetRightIndicator(this GTANetworkAPI.Vehicle vehicle, bool state)
        {
            vehicle.SetSharedData("V_RIGHT_LED", state);
        }
        
        public static bool GetLeftIndicator(this GTANetworkAPI.Vehicle vehicle)
        {
            return !vehicle.HasSharedData("V_LEFT_LED") || vehicle.GetSharedData<bool>("V_LEFT_LED");
        }
        
        public static void SetLeftIndicator(this GTANetworkAPI.Vehicle vehicle, bool state)
        {
            vehicle.SetSharedData("V_LEFT_LED", state);
        }
        
        public static void AddFuel(this GTANetworkAPI.Vehicle source, float fuel)
        {
            var vehicle = source.GetInfo();
            if(vehicle == null || !source.HasSharedData("V_FUEL"))
                throw new Exception($"SetFuel -> source info or shared does null");
            
            vehicle.Fuel += fuel;
            source.SetSharedData("V_FUEL", Convert.ToInt32(vehicle.Fuel));
            
            // Console.WriteLine($"Fuel of vehicle {vehicle.Id} = {vehicle.Fuel}/{vehicle.MaxFuel}");
        }

        public static float GetFuel(this GTANetworkAPI.Vehicle source)
        {
            var vehicle = source.GetInfo();
            return vehicle?.Fuel ?? 0.0f;
        }
    }
}