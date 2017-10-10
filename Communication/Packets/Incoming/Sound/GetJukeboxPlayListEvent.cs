using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing;

namespace Quasar.Communication.Packets.Incoming.Sound
{
    class GetJukeboxPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
                Session.SendMessage(new SetJukeboxPlayListComposer(Session.GetHabbo().CurrentRoom));
        }
    }
}
