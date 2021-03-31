using System.Linq;
using System.Numerics;
using API.Database;
using API.Profile;
using GM.Global;
using GTANetworkAPI;

namespace GM.Client
{
    public static class Util
    {
        public static Profile GetProfile(this Player player) => 
            !player.HasData(PlayerVariables.Profile) ? null : player.GetData<Profile>(PlayerVariables.Profile);

        public static void SetProfile(this Player player, Profile profile)
        {
            player.SetData(PlayerVariables.Profile, profile);
        }

        /// <summary>
        /// Есть ли у игрока доступ к административной команде
        /// </summary>
        /// <param name="player">игрок</param>
        /// <param name="command">команда</param>
        /// <returns></returns>
        public static bool DoesHaveCommandAccess(this Player player, string command)
        {
            var profile = player.GetProfile();
            if (profile == null)
                return false;

            var cmd = Pools.Commands.FirstOrDefault(x => x.Name == command);
            if (cmd == null)
                return false;

            return profile.AccessLevel >= cmd.AccessLevel;
        }

        public static void SetAccess(this Player player, int access)
        {
            var profile = player.GetProfile();
            if (profile == null)
                return;

            profile.AccessLevel = access;
            profile.Save();
            
            player.SetDebugData(PlayerVariables.AccessLevel, access);
            player.SetSharedData(PlayerVariables.AccessLevel, access);
            
            player.ShowNotify(Notify.MessageType.Success, "Notify", "Уровень доступа изменен!");
        }

        public static bool DoesPlayerDead(this Player player)
        {
            return player.HasSharedData(PlayerVariables.Dead) && player.GetSharedData<bool>(PlayerVariables.Dead);
        }
    }
}