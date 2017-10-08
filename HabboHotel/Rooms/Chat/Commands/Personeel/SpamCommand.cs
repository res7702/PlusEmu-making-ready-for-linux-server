#region

using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    internal class SpamcCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_spam"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Om | te spammen in de kamer."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
            Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "|", 0, ThisUser.LastBubble));
        }
    }
}