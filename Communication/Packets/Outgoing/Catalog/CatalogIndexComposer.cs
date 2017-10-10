using System.Collections.Generic;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient Session, ICollection<CatalogPage> Pages, int Sub = 0)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(Session, Pages);

            foreach (CatalogPage Page in Pages)
            {
                if (Page.ParentId != -1 || Page.MinimumRank > Session.GetHabbo().Rank)
                    continue;

                WritePage(Page, CalcTreeSize(Session, Pages, Page.Id));

                foreach (CatalogPage child in Pages)
                {
                    if (child.ParentId != Page.Id || Page.MinimumRank > Session.GetHabbo().Rank)
                        continue;

                    WritePage(child, CalcTreeSize(Session, Pages, child.Id));

                    foreach (CatalogPage child2 in Pages)
                    {
                        if (child2.ParentId != child.Id || child.MinimumRank > Session.GetHabbo().Rank)
                            continue;
                        WritePage(child2, 0);

                    }
                 }
            }
      

            base.WriteBoolean(false);
            base.WriteString("NORMAL");
        }

        public void WriteRootIndex(GameClient Session, ICollection<CatalogPage> Pages)
        {
            base.WriteBoolean(true);
            base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteString("root");
            base.WriteString(string.Empty);
            base.WriteInteger(0);
            base.WriteInteger(CalcTreeSize(Session, Pages, -1));
        }

        public void WritePage(CatalogPage Page, int TreeSize)
        {
            base.WriteBoolean(Page.Visible);
            base.WriteInteger(Page.Icon);
            base.WriteInteger(!Page.Enabled ? -1 : Page.Id);
            base.WriteString(Page.PageLink);
            base.WriteString(Page.Caption);
            base.WriteInteger(Page.ItemOffers.Count);
            foreach (int i in Page.ItemOffers.Keys)
            {
                base.WriteInteger(i);
            }
            base.WriteInteger(TreeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.GetHabbo().Rank || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }
    }
}