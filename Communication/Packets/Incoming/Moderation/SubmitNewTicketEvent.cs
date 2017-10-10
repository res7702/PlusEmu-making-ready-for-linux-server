using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Support;
using Quasar.HabboHotel.Rooms.Chat.Moderation;
using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.Communication.Packets.Incoming.Moderation
{
    class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (QuasarEnvironment.GetGame().GetModerationTool().UsersHasPendingTicket(Session.GetHabbo().Id))
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Je hebt op dit moment nog een ticket ingestuurd. Wacht even tot deze behandeld is!"));
                return;
            }

            string Message = Packet.PopString();
            int Type = Packet.PopInt();
            int ReportedUser = Packet.PopInt();
            int Room = Packet.PopInt();

            int Messagecount = Packet.PopInt();
            List<string> Chats = new List<string>();
            for (int i = 0; i < Messagecount; i++)
            {
                Packet.PopInt();
                Chats.Add(Packet.PopString());
            }

            ModerationRoomChatLog Chat = new ModerationRoomChatLog(Packet.PopInt(), Chats);

            QuasarEnvironment.GetGame().GetModerationTool().SendNewTicket(Session, Type, ReportedUser, Message, Chats);
            QuasarEnvironment.GetGame().GetClientManager().ModAlert("Je nieuwe ticket is aangemaakt!");

        }
    }
}
