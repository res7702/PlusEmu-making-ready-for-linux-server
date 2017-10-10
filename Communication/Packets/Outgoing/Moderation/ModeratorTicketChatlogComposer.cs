using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Support;

namespace Quasar.Communication.Packets.Outgoing.Moderation
{
    class ModeratorTicketChatlogComposer : ServerPacket
    {
        public ModeratorTicketChatlogComposer(SupportTicket Ticket, RoomData RoomData, double Timestamp)
            : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {
            base.WriteInteger(Ticket.TicketId);
            base.WriteInteger(Ticket.SenderId);
            base.WriteInteger(Ticket.ReportedId);
            base.WriteInteger(RoomData.Id);

            base.WriteByte(1);
            base.WriteShort(2);//Count
            base.WriteString("roomName");
            base.WriteByte(2);
            base.WriteString(RoomData.Name);
            base.WriteString("roomId");
            base.WriteByte(1);
            base.WriteInteger(RoomData.Id);

            base.WriteShort(Ticket.ReportedChats.Count);
            foreach (string Chat in Ticket.ReportedChats)
            {
                Habbo Habbo = QuasarEnvironment.GetHabboById(Ticket.ReportedId);

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(Ticket.Timestamp).ToLocalTime();

                base.WriteString(dtDateTime.Hour + ":" + dtDateTime.Minute);
                base.WriteInteger(Ticket.ReportedId);
                base.WriteString(Habbo != null ? Habbo.Username : "Geen gebruiker");
                base.WriteString(Chat);
                base.WriteBoolean(false);

                //DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                //dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Log["timestamp"])).ToLocalTime();

                //base.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                //base.WriteInteger(Habbo.Id);
                //base.WriteString(Habbo.Username);
                //base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Log["message"])) ? "*stemen*" : Convert.ToString(Log["message"]));
                //base.WriteBoolean(false);
            }
        }
    }
}
