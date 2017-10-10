using System;
using Quasar.Communication.Packets.Incoming;

using Quasar.Utilities;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Users.Inventory;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Items.Utilities;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Catalog.Utilities;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

            if (QuasarEnvironment.GetDBConfig().DBData["gifts_enabled"] != "1")
            {
                Session.SendNotification("Het sturen van cadeau's is tijdelijk uitgeschakeld door het Hotel Management.");
                return;
            }

            /*if (QuasarEnvironment.GetGame().GetCatalog().CatalogFlatOffers.ContainsKey(ItemId) && PageId < 0)
            {
                PageId = QuasarEnvironment.GetGame().GetCatalog().CatalogFlatOffers[ItemId];

                CatalogPage P = null;
                if (!QuasarEnvironment.GetGame().GetCatalog().Pages.TryGetValue(PageId, out P))
                    PageId = 0;
            }*/

            CatalogPage Page = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if ( !Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank) /*|| (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))*/
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = (CatalogItem)Page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (!ItemUtility.CanGiftItem(Item))
                return;

            ItemData PresentData = null;
            if (!QuasarEnvironment.GetGame().GetItemManager().GetGift(SpriteId, out PresentData) || PresentData.InteractionType != InteractionType.GIFT)
                return;

            if (Session.GetHabbo().Credits < Item.CostCredits)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(true, false));
                return;
            }

            if (Session.GetHabbo().Duckets < Item.CostPixels)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(false, true));
                return;
            }

            Habbo Habbo = QuasarEnvironment.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                Session.SendMessage(new GiftWrappingErrorComposer());
                return;
            }

            if (!Habbo.AllowGifts)
            {
                Session.SendNotification("Oeps! Je kan geen cadeau's sturen naar deze Habbis.");
                return;
            }

            if (Session.GetHabbo().Rank < 4)
            {
                if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 10.0)
                {
                    Session.SendNotification("Oeps! Wacht minstens 10 seconden tussen het kopen van cadeau's.");
                    return;
                }
            }
                

            if (Session.GetHabbo().SessionGiftBlocked)
                return;


            string ED = GiftUser + Convert.ToChar(5) + GiftMessage + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + SpriteId + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                //Insert the dummy item.
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + PresentData.Id + "', '" + Habbo.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                   
                    #region Pet handling

                    case InteractionType.pet0:  // Hond
                    case InteractionType.pet1:  // Kat
                    case InteractionType.pet2:  // Krokodillen
                    case InteractionType.pet3:  // Terriers
                    case InteractionType.pet4:  // Beren
                    case InteractionType.pet5:  // Varkens
                    case InteractionType.pet6:  // Leeuwen
                    case InteractionType.pet7:  // Neushoorns
                    case InteractionType.pet8:  // Spinnen
                    case InteractionType.pet9:  // Schildpadden
                    case InteractionType.pet10: // Kuikens
                    case InteractionType.pet11: // Kikkers
                    case InteractionType.pet12: // Draken
                    case InteractionType.pet13: // Slenderman
                    case InteractionType.pet14: // Apen
                    case InteractionType.pet15: // Paarden
                    case InteractionType.pet16: // Monsterplanten
                    case InteractionType.pet17: // Konijnen
                    case InteractionType.pet18: // Evil Konijnen
                    case InteractionType.pet19: // Depressieve Konijnen
                    case InteractionType.pet20: // Liefdes Konijnen
                    case InteractionType.pet21: // Witte Duiven
                    case InteractionType.pet22: // Zwarte Duiven
                    case InteractionType.pet23: // Rode Aap
                    case InteractionType.pet24: // Baby Beertjes
                    case InteractionType.pet25: // Baby Terriers
                    case InteractionType.pet26: // Kabouters
                    case InteractionType.pet27: // Baby's
                    case InteractionType.pet28: // Baby Beertjes
                    case InteractionType.pet29: // Baby Terriers 
                    case InteractionType.pet30: // Kabouters
                    case InteractionType.pet31: // Oempa Loempa's
                    case InteractionType.pet32: // Stenen
                    case InteractionType.pet33: // Pterodactylussen
                    case InteractionType.pet34: // Velociraptors
                    case InteractionType.pet35: // Wolven
                    case InteractionType.pet36: // Monster Konijnen
                    case InteractionType.pet37: // Pickachu
                    case InteractionType.pet38: // Pinguins
                    case InteractionType.pet39: // Mario
                    case InteractionType.pet40: // Olifanten
                    case InteractionType.pet41: // Alien Konijnen
                    case InteractionType.pet42: // Gouden Konijnen
                    case InteractionType.pet43: // Roze Mewtwo
                    case InteractionType.pet44: // Entei
                    case InteractionType.pet45: // Blauwe Mewtwo
                    case InteractionType.pet46: // Cavia
                    case InteractionType.pet47: // Uil
                    case InteractionType.pet48: // Goude Mewtwo
                    case InteractionType.pet49: // Eend
                    case InteractionType.pet50: // Baby Bruin
                    case InteractionType.pet51: // Baby Wit
                    case InteractionType.pet52: // Dino
                    case InteractionType.pet53: // Yoshi
                    case InteractionType.pet54: // Koe
                    case InteractionType.pet55: // Pokémon: Gengar
                    case InteractionType.pet56: // Pokémon: Gengar
                    case InteractionType.pet57: // Pokémon: Gengar
                    case InteractionType.pet58: // Pokémon: Gengar
                    case InteractionType.pet59: // Pokémon: Gengar
                    case InteractionType.pet60: // Pokémon: Gengar
                    case InteractionType.pet61: // Pokémon: Gengar
                    case InteractionType.pet62: // Pokémon: Gengar
                    case InteractionType.pet63: // Pokémon: Gengar
                    case InteractionType.pet64: // Pokémon: Gengar
                    case InteractionType.pet65: // Pokémon: Gengar
                    case InteractionType.pet66: // Pokémon: Gengar
                    case InteractionType.pet67: // Pokémon: Gengar
                    case InteractionType.pet68: // Pokémon: Gengar
                    case InteractionType.pet69: // Pokémon: Gengar
                    case InteractionType.pet70: // Pokémon: Gengar
                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (PetUtility.CheckPetName(PetName))
                                return;

                            if (Race.Length > 2)
                                return;

                            if (Color.Length != 6)
                                return;

                            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        }
                        catch
                        {
                            return;
                        }

                        break;

                    #endregion

                    case InteractionType.FLOOR:
                    case InteractionType.WALLPAPER:
                    case InteractionType.LANDSCAPE:

                        Double Number = 0;
                        try
                        {
                            if (string.IsNullOrEmpty(Data))
                                Number = 0;
                            else
                                Number = Double.Parse(Data, QuasarEnvironment.CultureInfo);
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";
                        break;

                    case InteractionType.BADGE_DISPLAY:
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                        {
                            Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Je kan deze badge niet in een vitrine zetteno omdat je deze badge niet in je bezit hebt."));
                            return;
                        }

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Item.Data.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                //Here we're clearing up a record, this is dumb, but okay.
                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = " + NewItemId + " LIMIT 1;");
               }


            Item GiveItem = ItemFactory.CreateGiftItem(PresentData, Habbo, ED, ED, NewItemId, 0, 0);
            if (GiveItem != null)
            {
                GameClient Receiver = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    if (Receiver.GetHabbo().Rank <= 5)
                    {

                     Session.SendMessage(new RoomCustomizedAlertComposer("Je hebt een cadeau ontvangen van " + Session.GetHabbo().Username + "."));
                   
                    }
                        {

                        Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                        Receiver.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                        Receiver.SendMessage(new PurchaseOKComposer());
                        Receiver.SendMessage(new FurniListAddComposer(GiveItem));
                        Receiver.SendMessage(new FurniListUpdateComposer());

                    }
                }
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                    QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.GIFT_OTHERS);
                    Session.SendMessage(new RoomCustomizedAlertComposer("Je cadeau is ingepakt en verzonden!"));

            }
       
            Session.SendMessage(new PurchaseOKComposer(Item, PresentData));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}