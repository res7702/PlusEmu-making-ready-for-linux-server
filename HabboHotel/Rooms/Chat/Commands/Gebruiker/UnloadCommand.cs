#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class UnloadCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_unload"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Unload de huidige kamer."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                Room R = null;
                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Room.Id, out R))
                    return;

                QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(R, true);
            }
            else
            {
                if (Room.CheckRights(Session, true))
                {
                   QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);
                }
            }
        }
    }
}