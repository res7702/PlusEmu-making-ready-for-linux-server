#region

using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class DraaiCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_draai"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Dubbelklik om te draaien."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Room.CheckRights(Session) || User == null)
                return;

            User.ClickRotate = !User.ClickRotate;
            Session.SendMessage(new WhisperComposer(User.VirtualId, "Dubbelklik om te draaien is " + (User.ClickRotate ? "ingeschakeld - Herhaal de command om het uit te zetten" : "uitgeschakeld") + ".", 0, User.LastBubble));
        }
    }
}