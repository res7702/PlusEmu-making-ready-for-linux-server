using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new GiftWrappingConfigurationComposer());
        }
    }
}