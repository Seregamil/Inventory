using GM.Global;
using GTANetworkAPI;

namespace GM.Property.Vehicle
{
    public class KeyHandler : Script
    {
        [ServerEvent(Event.PlayerEnterVehicleAttempt)]
        public void PlayerEnterVehicleAttempt(Player player, GTANetworkAPI.Vehicle vehicle, sbyte seat)
        {
            if(player.DoesHaveKey(vehicle) && seat == 0)
            {
                player.TriggerEvent("Player:LoadVehicle", vehicle, true);
            }
        }
        
        [ServerEvent(Event.PlayerExitVehicleAttempt)]
        public void PlayerExitVehicle(Player player, GTANetworkAPI.Vehicle vehicle)
        {
            var data = vehicle.GetInfo();
            if(data == null)
                return;
            
            if(data.Id == -1)
                return;
            
            vehicle.Save();
        } 
    }
}