#region

using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class BouwHoogteCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bouwhoogte"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Bouw in de lucht zonder stacktile."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Room.CheckRights(Session) || User == null)
                return;

            if (Params.Length == 1)
            {
                User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Bouwhoogte is uitgezet.", 0, User.LastBubble));
                User.BuildHeight = null;
                return;
            }

            double height;
            if (double.TryParse(Params[1], out height))
            {
                User.BuildHeight = height;
                User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Bouwhoogte veranderd naar " + User.BuildHeight + ".(Gebruik de command :bh zonder nummer om het weer naar normaal te zetten.)", 0, User.LastBubble));
                User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Tip: Draaien gaat soms lastig. Gebruik daarom de command :draai om het bouwen nog aangenamer te maken.", 0, User.LastBubble));
            }
            else
            {
                User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Vul een nummer in, bijvoorbeeld :bh 10 of :bh 1.5", 0, User.LastBubble));
            }
        }
    }
}