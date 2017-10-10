using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;

namespace Quasar.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();
        
            Session.GetHabbo().PrepareRoom(RoomId, Password);
            
        }
    }
}