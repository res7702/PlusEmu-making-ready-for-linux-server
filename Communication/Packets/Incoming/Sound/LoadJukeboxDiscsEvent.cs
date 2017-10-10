using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.Communication.Packets.Outgoing;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Incoming.Sound
{
    class LoadJukeboxDiscsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
                Session.SendMessage(new LoadJukeboxUserMusicItemsComposer(Session.GetHabbo().CurrentRoom));
        }
    }
}
