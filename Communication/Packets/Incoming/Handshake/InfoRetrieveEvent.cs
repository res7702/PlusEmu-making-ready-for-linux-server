using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.Communication.Packets.Incoming.Handshake
{
    public class InfoRetrieveEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
            Session.SendMessage(new UserPerksComposer(Session.GetHabbo()));
        }
    }
}