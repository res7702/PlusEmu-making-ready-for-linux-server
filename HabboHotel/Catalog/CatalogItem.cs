using System;
using System.Collections.Generic;
using System.Data;
using Quasar.Core;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Incoming;
using Quasar.Communication.Packets.Outgoing;

namespace Quasar.HabboHotel.Catalog
{
    public class CatalogItem
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ItemData Data { get; set; }
        public int Amount { get; set; }
        public int CostCredits { get; set; }
        public string ExtraData { get; set; }
        public bool HaveOffer { get; set; }
        public bool IsLimited { get; set; }
        public string Name { get; set; }
        public int PageID { get; set; }
        public int CostPixels { get; set; }
        public int CostGOTWPoints { get; set; }
        public int CostPumpkins { get; set; }
        public int LimitedEditionStack { get; set; }
        public int LimitedEditionSells { get; set; }
        public int CostDiamonds { get; set; }
        public string Badge { get; set; }
        public int OfferId { get; set; }
        public int PredesignedId { get; set; }

        public CatalogItem(int Id, int ItemId, ItemData Data, string CatalogName, int PageId, int CostCredits, int CostPixels,
            int CostDiamonds, int Amount, int LimitedEditionSells, int LimitedEditionStack, bool HaveOffer, string ExtraData, string Badge, int OfferId, int CostGOTWPoints, int CostPumpkins, int PredesignedId)
        {
            this.Id = Id;
            this.Name = CatalogName;
            this.ItemId = ItemId;
            this.Data = Data;
            this.PageID = PageId;
            this.CostCredits = CostCredits;
            this.CostPixels = CostPixels;
            this.CostDiamonds = CostDiamonds;
            this.CostGOTWPoints = CostGOTWPoints;
            this.CostPumpkins = CostPumpkins;
            this.Amount = Amount;
            this.LimitedEditionSells = LimitedEditionSells;
            this.LimitedEditionStack = LimitedEditionStack;
            this.IsLimited = (LimitedEditionStack > 0);
            this.HaveOffer = HaveOffer;
            this.ExtraData = ExtraData;
            this.Badge = Badge;
            this.OfferId = OfferId;
            this.PredesignedId = PredesignedId;
        }

        internal ItemData GetBaseItem(int ItemIds)
        {
            ItemData obj = null;
            if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(ItemIds, out obj))
                return null;

            if (obj == null && this.Name != "room_ad_Quasar_badge")
                Console.WriteLine("UNKNOWN ItemIds: " + ItemIds);

            return obj;
        }

        internal void SerializeClub(ServerPacket Message, GameClients.GameClient Session)
        {
            Message.WriteInteger(Id);
            Message.WriteString(Name);
            Message.WriteBoolean(false);
            Message.WriteInteger(CostCredits);
            Message.WriteInteger((CostDiamonds > 0) ? CostDiamonds : CostDiamonds);
            Message.WriteInteger((CostDiamonds > 0) ? 105 : 0);
            Message.WriteBoolean(true); // don't know
            int Days = 0;
            int Months = 0;

            if (GetBaseItem(ItemId).InteractionType == InteractionType.club_1_month || GetBaseItem(ItemId).InteractionType == InteractionType.club_3_month || GetBaseItem(ItemId).InteractionType == InteractionType.club_6_month)
            {

                switch (GetBaseItem(ItemId).InteractionType)
                {
                    case InteractionType.club_1_month:
                        Months = 1;
                        break;

                    case InteractionType.club_3_month:
                        Months = 3;
                        break;

                    case InteractionType.club_6_month:
                        Months = 6;
                        break;
                }

                Days = 31 * Months;
            }

            DateTime future = DateTime.Now;
            if (Session.GetHabbo().GetClubManager().HasSubscription("habbo_vip"))
            {
                Double Expire = Session.GetHabbo().GetClubManager().GetSubscription("habbo_vip").ExpireTime;
                Double TimeLeft = Expire - QuasarEnvironment.GetUnixTimestamp();
                int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
                future = DateTime.Now.AddDays(TotalDaysLeft);
            }

            future = future.AddDays(Days);

            Message.WriteInteger(Months); // months
            Message.WriteInteger(Days); // days
            Message.WriteBoolean(true);
            Message.WriteInteger(Days); // wtf
            Message.WriteInteger(future.Year); // year
            Message.WriteInteger(future.Month); // month
            Message.WriteInteger(future.Day); // day
        }
        public int ExtradataInt
        {
            get
            {
                int num;
                bool flag = !int.TryParse(this.ExtraData, out num);
                int result;
                if (flag)
                {
                    result = 0;
                }
                else
                {
                    result = num;
                }
                return result;
            }
        }
    }
}