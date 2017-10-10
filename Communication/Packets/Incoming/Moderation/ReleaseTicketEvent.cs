﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Incoming.Moderation
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            int Amount = Packet.PopInt();
            for (int i = 0; i < Amount; i++)
            {
                int TicketId = Packet.PopInt();
                QuasarEnvironment.GetGame().GetModerationTool().ReleaseTicket(Session, TicketId);
            }
        }
    }
}
