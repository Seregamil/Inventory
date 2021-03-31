using GM.Global;
using GM.Property.Vehicle.Sync;
using GTANetworkAPI;

namespace GM.Property.Vehicle
{
    public class Events : Script
    {
        [RemoteEvent("Vehicle:Engine")]
        public void TurnEngine(Player player, GTANetworkAPI.Vehicle source)
        {
            if (source.GetInfo() == null)
                return;
            
            var state = source.GetEngineState();
            player.ShowNotify(Notify.MessageType.Success, "Engine", $"Engine state is {!state}");
            
            source.SetEngineState(!state);
        }
        
        [RemoteEvent("Vehicle:Locked")]
        public void vehicleInteract_Locked(Player player, GTANetworkAPI.Vehicle vehicle) 
        {
            if (vehicle.GetInfo() == null)
                return;

            if (!player.DoesHaveKey(vehicle))
            {
                player.ShowNotify(Notify.MessageType.Alert, "Error", "У вас нет ключей от авто!");
                return;
            }

            var state = vehicle.GetLockState();
            player.ShowNotify(Notify.MessageType.Success, "Lock", $"Lock state is {!state}");
            
            vehicle.SetLockState(!state);
        }
        
        [RemoteEvent("Vehicle:StopSignal")]
        public void vehicleInteract_StopSignal(Player player, GTANetworkAPI.Vehicle vehicle) 
        {
            if (vehicle.GetInfo() == null)
                return;
                
            if (!vehicle.HasSharedData("V_LEFT_LED") || !vehicle.HasSharedData("V_RIGHT_LED"))
                return;

            var right = vehicle.GetRightIndicator();
            var left = vehicle.GetLeftIndicator();

            if (right != left)
            {
                player.ShowNotify(Notify.MessageType.Alert, "Error", "Выключите поворотник");
                return;
            }

            vehicle.SetRightIndicator(!right);
            vehicle.SetLeftIndicator(!left);
        }

        [RemoteEvent("Vehicle:Left")]
        public void vehicleInteract_Left(Player player, GTANetworkAPI.Vehicle vehicle)
        {
            if (vehicle.GetInfo() == null)
                return;
                
            if (!vehicle.HasSharedData("V_LEFT_LED") || !vehicle.HasSharedData("V_RIGHT_LED"))
                return;
            
            var left = vehicle.GetLeftIndicator();

            if (vehicle.GetRightIndicator() && left)
            {
                player.ShowNotify(Notify.MessageType.Alert, "Error", "Выключите аварийку!");
                return;
            }
            
            vehicle.SetLeftIndicator(!left);
        }
        
        [RemoteEvent("Vehicle:Right")]
        public void vehicleInteract_Right(Player player, GTANetworkAPI.Vehicle vehicle)
        {
            if (vehicle.GetInfo() == null)
                return;
                
            if (!vehicle.HasSharedData("V_LEFT_LED") || !vehicle.HasSharedData("V_RIGHT_LED"))
                return;
            
            var right = vehicle.GetRightIndicator();

            if (vehicle.GetLeftIndicator() && right)
            {
                player.ShowNotify(Notify.MessageType.Alert, "Error", "Выключите аварийку!");
                return;
            }
            
            vehicle.SetRightIndicator(!right);
        }
        
        [RemoteEvent("Vehicle:Handbrake")]
        public void vehicleInteract_Handbrake(Player player, GTANetworkAPI.Vehicle vehicle) 
        {
            if (vehicle.GetInfo() == null)
                return;

            var state = vehicle.GetHandbrakeState();
            player.ShowNotify(Notify.MessageType.Success, "Handbrake", $"Handbrake state is {!state}");
            
            vehicle.SetHandbrakeState(!state);
        }

        [RemoteEvent("Vehicle:Door")]
        public void vehicleInteract_Door(Player player, GTANetworkAPI.Vehicle vehicle, int index) 
        {
            if (vehicle.GetInfo() == null)
                return;

            var doorState = vehicle.GetDoorState(index);
            if (doorState == DoorState.Broken)
                return;

            vehicle.SetDoorState(index,
                doorState == DoorState.Open ? DoorState.Closed : DoorState.Open);
        }
        
        [RemoteEvent("Vehicle:Trunk")]
        public void vehicleInteract_Trunk(Player player, GTANetworkAPI.Vehicle vehicle) 
        {
            if (vehicle.GetInfo() == null)
                return;
            
            var doorState = vehicle.GetDoorState(5);
            if (doorState == DoorState.Broken)
                return;

            vehicle.SetDoorState(5,
                doorState == DoorState.Open ? DoorState.Closed : DoorState.Open);
        }
        
        [RemoteEvent("Vehicle:Bonnet")]
        public void vehicleInteract_Bonnet(Player player, GTANetworkAPI.Vehicle vehicle) 
        {
            if (vehicle.GetInfo() == null)
                return;
            
            var doorState = vehicle.GetDoorState(4);
            if (doorState == DoorState.Broken)
                return;

            vehicle.SetDoorState(4,
                doorState == DoorState.Open ? DoorState.Closed : DoorState.Open);        
        }
    }
}