using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Catalog;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    class GetClubGiftsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new ClubGiftsComposer());
        }
    }
}
