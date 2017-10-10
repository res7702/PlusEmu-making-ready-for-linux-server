using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Rooms;

namespace Quasar.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int userId = Packet.PopInt();
            if (userId == 0 || userId == Session.GetHabbo().Id)
                return;
            
            string message = Packet.PopString();
            if (string.IsNullOrWhiteSpace(message)) return;
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Oeps! Je hebt een spreekverbod."));
                return;
            }

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(message, out word))
            {
                Room _room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(2);
                Session.GetHabbo().PrepareRoom(_room.Id, "2");
                Session.SendMessage(new RoomCustomizedAlertComposer("Het woord '" + word + "' is verboden."));
                QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("bubble_filter", "Filter bericht\n\nHabbis: " + Session.GetHabbo().Username + "\nWoord: " + word + "\nType: Chatconsole", ""));
                return;
            }

            Session.GetHabbo().GetMessenger().SendInstantMessage(userId, message);

        }
    }
}