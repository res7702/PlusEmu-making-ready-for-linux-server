using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new MarketplaceConfigurationComposer());
        }
    }
}