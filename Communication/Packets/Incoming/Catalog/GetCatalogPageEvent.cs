using System;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();

            CatalogPage Page = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            //else if (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1)
            //{
            //    Session.SendNotification("Oeps! Dit is een pagina enkel voor Premium-leden. Koop Premium via de website om ook van dit gedeelte te kunnen genieten.");
            //    return;
            //}

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank)
                return;

           Session.SendMessage(new CatalogPageComposer(Page, CataMode));
        }
    }
}