using Quasar.Communication.Packets.Outgoing.Campaigns;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Calendar
{
    class OpenCalendarBoxEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CampaignName = Packet.PopString();
            int CampaignDay = Packet.PopInt(); // INDEX VALUE.

            // Si no es el nombre de campaña actual.
            if (CampaignName != QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignName())
                return;

            // Si es un día inválido.
            if (CampaignDay < 0 || CampaignDay > QuasarEnvironment.GetGame().GetCalendarManager().GetTotalDays() - 1 || CampaignDay < QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays())
                // Mini fix
                return;



            // Días próximos
            if (CampaignDay > QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays())

                return;


            // Esta recompensa ya ha sido recogida.
            if (Session.GetHabbo().calendarGift[CampaignDay])

                return;


            Session.GetHabbo().calendarGift[CampaignDay] = true;

            // PACKET PARA ACTUALIZAR?
            Session.SendMessage(new CalendarPrizesComposer(QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignDay(CampaignDay + 1)));
            Session.SendMessage(new CampaignCalendarDataComposer(Session.GetHabbo().calendarGift));

            string Gift = QuasarEnvironment.GetGame().GetCalendarManager().GetGiftByDay(CampaignDay + 1);
            string GiftType = Gift.Split(':')[0];
            string GiftValue = Gift.Split(':')[1];

            switch (GiftType.ToLower())
            {
                case "itemid":
                    {
                        ItemData Item = null;
                        if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(int.Parse(GiftValue), out Item))
                        {
                            // No existe este ItemId.
                            return;
                        }

                        Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                        if (GiveItem != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                            Session.SendMessage(new FurniListUpdateComposer());
                            Session.SendNotification("Hoera! Je hebt een item ontvangen! (Open je inventaris).");
                            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Kalender", 1);
                        }

                        Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    break;

                case "badge":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(GiftValue, true, Session);
                        Session.SendNotification("Hoera! Je hebt een Badge ontvangen!");
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Kalender", 1);
                    }
                    break;

                case "diamonds":
                    {
                        Session.GetHabbo().Diamonds += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                        Session.SendNotification("Hoera! Je hebt " + GiftValue + " Diamanten ontvangen!");
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Kalender", 1);
                    }
                    break;

                case "gotwpoints":
                    {
                        Session.GetHabbo().GOTWPoints += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                        Session.SendNotification("Hoera! Je hebt " + GiftValue + " punten ontvangen!");
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Kalender", 1);
                    }
                    break;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("INSERT INTO user_campaign_gifts VALUES (NULL, '" + Session.GetHabbo().Id + "','" + CampaignName + "','" + (CampaignDay + 1) + "')");
            }
        }
    }
}
