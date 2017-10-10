using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Talents;
using Quasar.HabboHotel.Achievements;

namespace Quasar.Communication.Packets.Incoming.Talents
{
    class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            List<Talent> talents = QuasarEnvironment.GetGame().GetTalentManager().GetTalents(Type, -1);

            if (talents == null)
                return;

            Session.SendMessage(new TalentTrackComposer(Session, Type, talents));
        }
    }
}
