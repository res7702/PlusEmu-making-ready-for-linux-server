using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.Communication.Packets.Incoming.Handshake

{
    public class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.RC4Client == null || Session.GetHabbo() != null)
                return;

            string SSO = Packet.PopString();

            if (string.IsNullOrEmpty(SSO) /*|| SSO.Length < 15  TThis one depends on your CMS*/)
                return;

            Session.TryAuthenticate(SSO);
        }
    }
}
//{
//    public class SSOTicketEvent : IPacketEvent
//    {
//        public void Parse(GameClient Session, ClientPacket Packet)
//        {
//            if (Session == null || Session.RC4Client == null || Session.GetHabbo() != null)
//                return;

//            Session.TryAuthenticate(Packet.PopString());
//        }
//    }
//}