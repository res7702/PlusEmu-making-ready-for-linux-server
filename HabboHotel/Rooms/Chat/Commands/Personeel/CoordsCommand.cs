#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    internal class CoordsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_coords"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Bekijk de coördinaten van waar je op dit moment staat."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            Session.SendNotification("X: " + ThisUser.X + "\nY: " + ThisUser.Y + "\nZ: " + ThisUser.Z + "\n\nRot: " +
                                     ThisUser.RotBody + ", sqState: " +
                                     Room.GetGameMap().GameMap[ThisUser.X, ThisUser.Y] + "\n\nKamer[id]: " +
                                     Session.GetHabbo().CurrentRoomId);
        }
    }
}