using System;
using System.Linq;
using GTANetworkAPI;
using System.Timers;
using GM.Property.Vehicle.Sync;

namespace GM.Property.Vehicle
{
    public class FuelCounter : Script
    {
        private const float FuelValue = -0.02f;
        
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            var fuelTimer = new Timer(500);
            fuelTimer.Elapsed += (sender, args) =>
            {
                NAPI.Task.Run(() =>
                {
                    foreach (var source in NAPI.Pools.GetAllVehicles()
                        .Where(x => x.GetInfo() != null && x.GetInfo().Id != -1 && x.GetEngineState()))
                    {
                        if (source.GetFuel() <= FuelValue)
                        {
                            if(source.GetEngineState())
                                source.SetEngineState(false);
                            
                            continue;
                        }

                        source.AddFuel(FuelValue);
                    }
                });
            };
            
            fuelTimer.Start();
        }

        [Command("refuel")]
        public void RefuelCMD(Player player, int litres)
        {
            var source = player.Vehicle;
            
            if(source == null)
                return;

            var vehicle = source.GetInfo();
            if(vehicle == null)
                return;

            if(vehicle.Fuel + litres > vehicle.MaxFuel)
                source.AddFuel(vehicle.MaxFuel - vehicle.Fuel);
            else
                source.AddFuel(litres);
        }
    }
}