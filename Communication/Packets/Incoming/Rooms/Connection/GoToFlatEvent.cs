using System;
using System.Linq;
using System.Text;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;

namespace Quasar.Communication.Packets.Incoming.Rooms.Connection
{
    class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            if (!Session.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
                Session.SendMessage(new CloseConnectionComposer());
        }
    }
}
