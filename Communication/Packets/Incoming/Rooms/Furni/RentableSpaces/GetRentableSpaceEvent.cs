using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Rooms.Furni.RentableSpaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni.RentableSpaces
{
    class GetRentableSpaceEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int Something = Packet.PopInt();
            Session.SendMessage(new RentableSpaceComposer());
        }
    }
}
