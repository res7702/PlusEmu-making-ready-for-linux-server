using System;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.Communication.Packets.Incoming.Handshake
{
    public class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Junk = Packet.PopString();
            string MachineId = Packet.PopString();

            Session.MachineId = MachineId;

            Session.SendMessage(new SetUniqueIdComposer(MachineId));
        }
    }
}