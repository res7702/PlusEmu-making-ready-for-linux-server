using System;
using Quasar.Communication.Packets.Incoming;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.BuildersClub;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            /*int Sub = 0;

            if (Session.GetHabbo().GetSubscriptionManager().HasSubscription)
            {
                Sub = Session.GetHabbo().GetSubscriptionManager().GetSubscription().SubscriptionId;
            }*/

            Session.SendMessage(new CatalogIndexComposer(Session, QuasarEnvironment.GetGame().GetCatalog().GetPages()));//, Sub));
            Session.SendMessage(new CatalogItemDiscountComposer());
            Session.SendMessage(new BCBorrowedItemsComposer());
        }
    }
}