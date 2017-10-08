#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class WarpCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_warp"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Teleporteer door je kamer heen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Room.CheckRights(Session) || User == null)
                return;

            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();

        }
    }
}