using System;
using System.Linq;
using System.Collections.Generic;

using Quasar.Core;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Users.Effects;
using Quasar.HabboHotel.Items.Utilities;
using Quasar.HabboHotel.Users.Inventory.Bots;

using Quasar.HabboHotel.Rooms.AI;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Inventory.Bots;
using Quasar.Communication.Packets.Outgoing.Inventory.Pets;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Utilities;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Quests;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (QuasarEnvironment.GetDBConfig().DBData["catalogue_enabled"] != "1")
            {
                Session.SendNotification("De shop is tijdelijk uitgeschakeld wegens onderhoud.");
                return;
            }

            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string ExtraData = Packet.PopString();
            int Amount = Packet.PopInt();


            CatalogPage Page = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if (!Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank) /*|| (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))*/
                return;


            if (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1)
            {
                Session.SendNotification("Oeps! Enkel Premium-leden kunnen gebruik maken van de meubi's in deze categorie. Je kunt Premium kopen via de website!");
                return;
            }

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

            ItemData baseItem = Item.GetBaseItem(Item.ItemId);
            if (baseItem != null)
            {
                if (baseItem.InteractionType == InteractionType.club_1_month || baseItem.InteractionType == InteractionType.club_3_month || baseItem.InteractionType == InteractionType.club_6_month)
                {
                    if (Item.CostCredits > Session.GetHabbo().Credits)
                        return;

                    int Months = 0;

                    switch (baseItem.InteractionType)
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


                    int num = num = 31 * Months;

                    if (Item.CostCredits > 0)
                    {
                        Session.GetHabbo().Credits -= Item.CostCredits;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));

                    }

                    Session.GetHabbo().GetClubManager().AddOrExtendSubscription("habbo_vip", num * 24 * 3600, Session);
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipHC", 1);

                    Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
                    Session.SendMessage(new FurniListUpdateComposer());
                    return;
                 }

                if (baseItem.InteractionType == InteractionType.pet35)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_BuyPetWolf", 1, false);
                }

                if (baseItem.InteractionType == InteractionType.pet27)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreBaby", 1, false);
                }


                if (baseItem.InteractionType == InteractionType.pet50)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreBaby", 1, false);
                }


                if (baseItem.InteractionType == InteractionType.pet51)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreBaby", 1, false);
                }
              }

            if (Amount < 1 || Amount > 100)
                Amount = 1;

            int AmountPurchase = Item.Amount > 1 ? Item.Amount : Amount;
            int TotalCreditsCost = Amount > 1 ? ((Item.CostCredits * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostCredits)) : Item.CostCredits;
            int TotalPixelCost = Amount > 1 ? ((Item.CostPixels * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPixels)) : Item.CostPixels;
            int TotalDiamondCost = Amount > 1 ? ((Item.CostDiamonds * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostDiamonds)) : Item.CostDiamonds;
            int TotalGOTWPointsCost = Amount > 1 ? ((Item.CostGOTWPoints * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostGOTWPoints)) : Item.CostGOTWPoints;
            int TotalPumpkinsCost = Amount > 1 ? ((Item.CostPumpkins * Amount) - ((int)Math.Floor((double)Amount / 6) * Item.CostPumpkins)) : Item.CostPumpkins;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().Diamonds < TotalDiamondCost || Session.GetHabbo().GOTWPoints < TotalGOTWPointsCost || Session.GetHabbo().Pumpkins < TotalPumpkinsCost)
                return;

            int LimitedEditionSells = 0;
            int LimitedEditionStack = 0;


            if (Item.IsLimited)
            {
                if (Item.LimitedEditionStack <= Item.LimitedEditionSells)
                {
                    Session.SendNotification("Oeps! Habbis beschikt over te weinig exemplaren van dit Limited Edition item. Geen paniek de aankoop prijs is teruggestort! ");
                    Session.SendMessage(new CatalogUpdatedComposer());
                    Session.SendMessage(new PurchaseOKComposer());
                    return;
                }

               Item.LimitedEditionSells++;
               QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_LimitedEdition", 1);
               using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `catalog_items` SET `limited_sells` = '" + Item.LimitedEditionSells + "' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    LimitedEditionSells = Item.LimitedEditionSells;
                    LimitedEditionStack = Item.LimitedEditionStack;
                }
            }

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= TotalDiamondCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_KoopDiamanten", TotalDiamondCost);
            }

            if (Item.CostGOTWPoints > 0)
            {
                Session.GetHabbo().GOTWPoints -= TotalGOTWPointsCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103)); // Puntos de Honor Custom - Habbi.es
            }
           
            if (Item.CostPumpkins > 0)
            {
                Session.GetHabbo().Pumpkins -= TotalPumpkinsCost;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Pumpkins, 0, 104)); // Calabazas Custom - Habbi.es
            }
           



            #region PREDESIGNED_ROOM BY KOMOK
            if (Item.PredesignedId > 0 && QuasarEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.ContainsKey((uint)Item.PredesignedId))
            {
                #region SELECT ROOM AND CREATE NEW
                var predesigned = QuasarEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom[(uint)Item.PredesignedId];
                var decoration = predesigned.RoomDecoration;

                var createRoom = QuasarEnvironment.GetGame().GetRoomManager().CreateRoom(Session, Session.GetHabbo().Username + "'s kamer", "Een pre-designed kamer :)", predesigned.RoomModel, 1, 25, 1);
                
                createRoom.FloorThickness = int.Parse(decoration[0]);
                createRoom.WallThickness = int.Parse(decoration[1]);
                createRoom.Model.WallHeight = int.Parse(decoration[2]);
                createRoom.Hidewall = ((decoration[3] == "True") ? 1 : 0);
                createRoom.Wallpaper = decoration[4];
                createRoom.Landscape = decoration[5];
                createRoom.Floor = decoration[6];
                var newRoom = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(createRoom.Id);
                #endregion

                #region CREATE FLOOR ITEMS
                if (predesigned.FloorItems != null)
                    foreach (var floorItems in predesigned.FloorItemData)
                        using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + floorItems.BaseItem + ", '" + floorItems.ExtraData + "', " +
                                floorItems.X + ", " + floorItems.Y + ", " + TextHandling.GetString(floorItems.Z) + ", " + floorItems.Rot + ", '', 0, 0);");
                #endregion

                #region CREATE WALL ITEMS
                if (predesigned.WallItems != null)
                    foreach (var wallItems in predesigned.WallItemData)
                        using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("INSERT INTO items VALUES (null, " + Session.GetHabbo().Id + ", " + newRoom.RoomId + ", " + wallItems.BaseItem + ", '" + wallItems.ExtraData +
                                "', 0, 0, 0, 0, '" + wallItems.WallCoord + "', 0, 0);");
                #endregion

                #region VERIFY IF CONTAINS BADGE AND GIVE
                if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
                #endregion

                #region GENERATE ROOM AND SEND PACKET
                Session.SendMessage(new PurchaseOKComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(newRoom.Id).GetRoomItemHandler().LoadFurniture();
                var newFloorItems = newRoom.GetRoomItemHandler().GetFloor;
                foreach (var roomItem in newFloorItems) newRoom.GetRoomItemHandler().SetFloorItem(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ);
                var newWallItems = newRoom.GetRoomItemHandler().GetWall;
                foreach (var roomItem in newWallItems) newRoom.GetRoomItemHandler().SetWallItem(Session, roomItem);
                Session.SendMessage(new FlatCreatedComposer(newRoom.Id, newRoom.Name));
                #endregion
                return;
            }
            #endregion

            #region Create the extradata
            switch (Item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    ExtraData = "";
                    break;

                case InteractionType.MUSIC_DISC:
                    ExtraData = Item.ExtraData;
                    break;

                case InteractionType.GUILD_FORUM:
                    int GroupId;
                    if (!int.TryParse(ExtraData, out GroupId))
                        break;

                    Group gp;
                    if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out gp))
                        break;

                    if (gp.HasForum)
                        break;

                    var forum = QuasarEnvironment.GetGame().GetGroupForumManager().CreateGroupForum(gp);



                    Session.SendMessage(new RoomNotificationComposer("forums.delivered"));

                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                    break;


                case InteractionType.PINATA:
                case InteractionType.PINATATRIGGERED:
                case InteractionType.MAGICEGG:
                    ExtraData = "0";
                    break;

                case InteractionType.FOOTBALL_GATE:
                    ExtraData = "hd-180-14.ch-210-1408.lg-270-1408,hd-600-14.ch-630-1408.lg-695-1408";
                    break;

                #region Pet handling

                case InteractionType.pet0:  // Hond
                case InteractionType.pet1:  // Kat
                case InteractionType.pet2:  // Krokodil
                case InteractionType.pet3:  // Terrier
                case InteractionType.pet4:  // Beren
                case InteractionType.pet5:  // Varkens
                case InteractionType.pet6:  // Leeuwen
                case InteractionType.pet7:  // Neushoorns
                case InteractionType.pet8:  // Spinnen
                case InteractionType.pet9:  // Schildpadden
                case InteractionType.pet10: // Kuikens
                case InteractionType.pet11: // Kikkers
                case InteractionType.pet12: // Draken
                case InteractionType.pet13: // Monsters
                case InteractionType.pet14: // Apen
                case InteractionType.pet15: // Paarden
                case InteractionType.pet16: // Monsterplant
                case InteractionType.pet17: // Konijnen
                case InteractionType.pet18: // Evil Konijnen
                case InteractionType.pet19: // Depressieve Konijnen
                case InteractionType.pet20: // Liefdes Konijnen
                case InteractionType.pet21: // Witte Duiven
                case InteractionType.pet22: // Zwarte Duiven
                case InteractionType.pet23: // Bezeten Apen
                case InteractionType.pet24: // Baby Beertjes
                case InteractionType.pet25: // Baby Terriers
                case InteractionType.pet26: // Kabouters
                case InteractionType.pet27: // Baby's
                case InteractionType.pet28: // Baby Kittens 
                case InteractionType.pet29: // Baby Puppy's
                case InteractionType.pet30: // Baby Varkens
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
                        string[] Bits = ExtraData.Split('\n');
                        string PetName = Bits[0];
                        string Race = Bits[1];
                        string Color = Bits[2];

                        int.Parse(Race); // to trigger any possible errors

                        if (!PetUtility.CheckPetName(PetName))
                            return;

                        if (Race.Length > 2)
                            return;

                        if (Color.Length != 6)
                            return;

                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                    }
                    catch (Exception e)
                    {
                        Logging.LogException(e.ToString());
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
                        if (string.IsNullOrEmpty(ExtraData))
                            Number = 0;
                        else
                            Number = Double.Parse(ExtraData, QuasarEnvironment.CultureInfo);
                    }
                    catch (Exception e)
                    {
                        Logging.HandleException(e, "Catalog.HandlePurchase: " + ExtraData);
                    }

                    ExtraData = Number.ToString().Replace(',', '.');
                    break; // maintain extra data // todo: validate

                case InteractionType.POSTIT:
                    ExtraData = "FFFF33";
                    break;

                case InteractionType.MOODLIGHT:
                    ExtraData = "1,1,1,#000000,255";
                    break;

                case InteractionType.TROPHY:
                    ExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + ExtraData;
                    break;

                case InteractionType.MANNEQUIN:
                    ExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Maniqui";
                    break;

                case InteractionType.BADGE_DISPLAY:
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(ExtraData))
                    {
                        Session.SendMessage(new BroadcastMessageAlertComposer("Oeps! Je kan deze badge niet in een vitrine zetteno omdat je deze badge niet in je bezit hebt."));
                        return;
                    }

                    ExtraData = ExtraData + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                    break;

                case InteractionType.BADGE:
                    {
                        if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Data.ItemName))
                        {
                            Session.SendMessage(new PurchaseErrorComposer(1));
                            return;
                        }
                        break;
                    }
                default:
                    ExtraData = "";
                    break;
            }
            #endregion

            Item NewItem = null;
            switch (Item.Data.Type.ToString().ToLower())
            {
                default:
                    List<Item> GeneratedGenericItems = new List<Item>();

                    switch (Item.Data.InteractionType)
                    {
                        default:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, 0, LimitedEditionSells, LimitedEditionStack);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.GUILD_GATE:
                        case InteractionType.GUILD_ITEM:
                        case InteractionType.GUILD_FORUM:
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase, Convert.ToInt32(ExtraData));

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData, Convert.ToInt32(ExtraData));

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.MUSIC_DISC:
                            string flags = Convert.ToString(Item.ExtradataInt);
                            if (AmountPurchase > 1)
                            {
                                List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), flags, AmountPurchase);

                                if (Items != null)
                                {
                                    GeneratedGenericItems.AddRange(Items);
                                }
                            }
                            else
                            {
                                NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), flags, flags);

                                if (NewItem != null)
                                {
                                    GeneratedGenericItems.Add(NewItem);
                                }
                            }
                            break;

                        case InteractionType.ARROW:
                        case InteractionType.TELEPORT:
                            for (int i = 0; i < AmountPurchase; i++)
                            {
                                List<Item> TeleItems = ItemFactory.CreateTeleporterItems(Item.Data, Session.GetHabbo());

                                if (TeleItems != null)
                                {
                                    GeneratedGenericItems.AddRange(TeleItems);
                                }
                            }
                            break;

                        case InteractionType.MOODLIGHT:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateMoodlightData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateMoodlightData(NewItem);
                                    }
                                }
                            }
                            break;

                      case InteractionType.TONER:
                            {
                                if (AmountPurchase > 1)
                                {
                                    List<Item> Items = ItemFactory.CreateMultipleItems(Item.Data, Session.GetHabbo(), ExtraData, AmountPurchase);

                                    if (Items != null)
                                    {
                                        GeneratedGenericItems.AddRange(Items);
                                        foreach (Item I in Items)
                                        {
                                            ItemFactory.CreateTonerData(I);
                                        }
                                    }
                                }
                                else
                                {
                                    NewItem = ItemFactory.CreateSingleItemNullable(Item.Data, Session.GetHabbo(), ExtraData, ExtraData);

                                    if (NewItem != null)
                                    {
                                        GeneratedGenericItems.Add(NewItem);
                                        ItemFactory.CreateTonerData(NewItem);
                                    }
                                }
                            }
                            break;

                     case InteractionType.DEAL:
                            {
                                //Fetch the deal where the ID is this items ID.
                                var DealItems = (from d in Page.Deals.Values.ToList() where d.Id == Item.Id select d);

                                //This bit, iterating ONE item? How can I make this simpler
                                foreach (CatalogDeal DealItem in DealItems)
                                {
                                    //Here I loop the DealItems ItemDataList.
                                    foreach (CatalogItem CatalogItem in DealItem.ItemDataList.ToList())
                                    {
                                        List<Item> Items = ItemFactory.CreateMultipleItems(CatalogItem.Data, Session.GetHabbo(), "", AmountPurchase);

                                        if (Items != null)
                                        {
                                            GeneratedGenericItems.AddRange(Items);
                                        }
                                    }
                                }
                                break;
                            }

                    }

                    foreach (Item PurchasedItem in GeneratedGenericItems)
                    {
                        if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                        {
                            Session.SendMessage(new FurniListAddComposer(PurchasedItem)); // Dylan
                            Session.SendMessage(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                        }
                    }
                    break;

                case "e":
                    AvatarEffect Effect = null;

                    if (Session.GetHabbo().Effects().HasEffect(Item.Data.SpriteId))
                    {
                        Effect = Session.GetHabbo().Effects().GetEffectNullable(Item.Data.SpriteId);

                        if (Effect != null)
                        {
                            Effect.AddToQuantity();
                        }
                    }
                    else
                        Effect = AvatarEffectFactory.CreateNullable(Session.GetHabbo(), Item.Data.SpriteId, 3600);

                    if (Effect != null)  //&&Session.GetHabbo().Effects().TryAdd(Effect));
                    {
                        Session.SendMessage(new AvatarEffectAddedComposer(Item.Data.SpriteId, 3600));
                    }

                   Session.SendMessage(RoomNotificationComposer.SendBubble("effect", "Je gekochte effect zit in je kledingkast. (Relog als je hem niet ziet)", ""));
                   break;

                   case "r":
                    Bot Bot = BotUtility.CreateBot(Item.Data, Session.GetHabbo().Id);
                    if (Bot != null)
                    {
                        Session.GetHabbo().GetInventoryComponent().TryAddBot(Bot);
                        Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                        Session.SendMessage(new FurniListNotificationComposer(Bot.Id, 5));
                    }
                    else
                        Session.SendNotification("Oeps! Er is een onbekende fout gedetecteerd tijdens het kopen van de Bot, meld dit bij een Habbis medewerker.");
                    break;

                case "b":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Data.ItemName, true, Session);
                        Session.SendMessage(new FurniListNotificationComposer(0, 4));
                        break;
                    }

                case "p":
                    {
                        switch (Item.Data.InteractionType)
                        {
                            #region Pets
                            #region Pet 0
                            case InteractionType.pet0:
                                string[] PetData = ExtraData.Split('\n');
                                Pet GeneratedPet = PetUtility.CreatePet(Session.GetHabbo().Id, PetData[0], 0, PetData[1], PetData[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet);

                                break;
                            #endregion
                            #region Pet 1

                            case InteractionType.pet1:
                                var PetData1 = ExtraData.Split('\n');
                                var GeneratedPet1 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData1[0], 1, PetData1[1],
                                    PetData1[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet1);

                                break;

                            #endregion    
                            #region Pet 2

                            case InteractionType.pet2:
                                var PetData5 = ExtraData.Split('\n');
                                var GeneratedPet5 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData5[0], 2, PetData5[1],
                                    PetData5[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet5);

                                break;

                            #endregion
                            #region Pet 3

                            case InteractionType.pet3:
                                var PetData2 = ExtraData.Split('\n');
                                var GeneratedPet2 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData2[0], 3, PetData2[1],
                                    PetData2[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet2);

                                break;

                            #endregion
                            #region Pet 4

                            case InteractionType.pet4:
                                var PetData3 = ExtraData.Split('\n');
                                var GeneratedPet3 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData3[0], 4, PetData3[1],
                                    PetData3[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet3);

                                break;

                            #endregion
                            #region Pet 5

                            case InteractionType.pet5:
                                var PetData7 = ExtraData.Split('\n');
                                var GeneratedPet7 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData7[0], 5, PetData7[1],
                                    PetData7[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet7);

                                break;

                            #endregion
                            #region Pet 6

                            case InteractionType.pet6:
                                var PetData4 = ExtraData.Split('\n');
                                var GeneratedPet4 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData4[0], 6, PetData4[1],
                                    PetData4[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet4);

                                break;

                            #endregion 
                            #region Pet 7

                            case InteractionType.pet7:
                                var PetData6 = ExtraData.Split('\n');
                                var GeneratedPet6 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData6[0], 7, PetData6[1],
                                    PetData6[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet6);

                                break;

                            #endregion
                            #region Pet 8

                            case InteractionType.pet8:
                                var PetData8 = ExtraData.Split('\n');
                                var GeneratedPet8 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData8[0], 8, PetData8[1],
                                    PetData8[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet8);

                                break;

                            #endregion
                            #region Pet 9

                            case InteractionType.pet9:
                                var PetData9 = ExtraData.Split('\n');
                                var GeneratedPet9 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData9[0], 9, PetData9[1],
                                    PetData9[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet9);

                                break;

                            #endregion#region Pet 10
                            #region Pet 10
                            case InteractionType.pet10:
                                var PetData10 = ExtraData.Split('\n');
                                var GeneratedPet10 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData10[0], 10,
                                    PetData10[1], PetData10[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet10);

                                break;

                            #endregion
                            #region Pet 11

                            case InteractionType.pet11:
                                var PetData11 = ExtraData.Split('\n');
                                var GeneratedPet11 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData11[0], 11,
                                    PetData11[1], PetData11[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet11);

                                break;

                            #endregion
                            #region Pet 12

                            case InteractionType.pet12:
                                var PetData12 = ExtraData.Split('\n');
                                var GeneratedPet12 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData12[0], 12,
                                    PetData12[1], PetData12[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet12);

                                break;

                            #endregion
                            #region Pet 13

                            case InteractionType.pet13:
                                var PetData13 = ExtraData.Split('\n');
                                var GeneratedPet13 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData13[0], 13,
                                    PetData13[1], PetData13[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet13);

                                break;

                            #endregion
                            #region Pet 14

                            case InteractionType.pet14:
                                var PetData14 = ExtraData.Split('\n');
                                var GeneratedPet14 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData14[0], 14,
                                    PetData14[1], PetData14[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet14);

                                break;

                            #endregion
                            #region Pet 15

                            case InteractionType.pet15:
                                var PetData15 = ExtraData.Split('\n');
                                var GeneratedPet15 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData15[0], 15,
                                    PetData15[1], PetData15[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet15);

                                break;

                            #endregion
                            #region Pet 16

                            case InteractionType.pet16:
                                var PetData16 = ExtraData.Split('\n');
                                var GeneratedPet16 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData16[0], 16,
                                    PetData16[1], PetData16[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet16);

                                break;

                            #endregion
                            #region Pet 17


                            case InteractionType.pet17:
                                var PetData17 = ExtraData.Split('\n');
                                var GeneratedPet17 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData17[0], 17,
                                    PetData17[1], PetData17[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet17);

                                break;

                            #endregion
                            #region Pet 18

                            case InteractionType.pet18:
                                var PetData18 = ExtraData.Split('\n');
                                var GeneratedPet18 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData18[0], 18,
                                    PetData18[1], PetData18[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet18);

                                break;

                            #endregion 
                            #region Pet 19

                            case InteractionType.pet19:
                                var PetData19 = ExtraData.Split('\n');
                                var GeneratedPet19 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData19[0], 19,
                                    PetData19[1], PetData19[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet19);

                                break;

                            #endregion
                            #region Pet 20

                            case InteractionType.pet20:
                                var PetData20 = ExtraData.Split('\n');
                                var GeneratedPet20 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData20[0], 20,
                                    PetData20[1], PetData20[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet20);

                                break;

                            #endregion
                            #region Pet 21

                            case InteractionType.pet21:
                                var PetData21 = ExtraData.Split('\n');
                                var GeneratedPet21 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData21[0], 21,
                                    PetData21[1], PetData21[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet21);

                                break;

                            #endregion
                            #region Pet 22

                            case InteractionType.pet22:
                                var PetData22 = ExtraData.Split('\n');
                                var GeneratedPet22 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData22[0], 22,
                                    PetData22[1], PetData22[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet22);

                                break;

                            #endregion
                            #region Pet 23

                            case InteractionType.pet23:
                                var PetData23 = ExtraData.Split('\n');
                                var GeneratedPet23 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData23[0], 23,
                                    PetData23[1], PetData23[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet23);

                                break;

                            #endregion
                            #region Pet 24

                            case InteractionType.pet24:
                                var PetData24 = ExtraData.Split('\n');
                                var GeneratedPet24 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData24[0], 24,
                                    PetData24[1], PetData24[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet24);

                                break;

                            #endregion
                            #region Pet 25

                            case InteractionType.pet25:
                                var PetData25 = ExtraData.Split('\n');
                                var GeneratedPet25 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData25[0], 25,
                                    PetData25[1], PetData25[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet25);

                                break;

                            #endregion
                            #region Pet 26

                            case InteractionType.pet26:
                                var PetData26 = ExtraData.Split('\n');
                                var GeneratedPet26 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData26[0], 26,
                                    PetData26[1], PetData26[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet26);

                                break;

                            #endregion         #region Pet 27
                            #region Pet 27
                            case InteractionType.pet27:
                                var PetData27 = ExtraData.Split('\n');
                                var GeneratedPet27 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData27[0], 27,
                                    PetData27[1], PetData27[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet27);

                                break;

                            #endregion
                            #region Pet 28

                            case InteractionType.pet28:
                                var PetData28 = ExtraData.Split('\n');
                                var GeneratedPet28 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData28[0], 28,
                                    PetData28[1], PetData28[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet28);

                                break;

                            #endregion
                            #region Pet 29

                            case InteractionType.pet29:
                                var PetData29 = ExtraData.Split('\n');
                                var GeneratedPet29 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData29[0], 29,
                                    PetData29[1], PetData29[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet29);

                                break;

                            #endregion
                            #region Pet 30

                            case InteractionType.pet30:
                                var PetData30 = ExtraData.Split('\n');
                                var GeneratedPet30 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData30[0], 30,
                                    PetData30[1], PetData30[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet30);

                                break;

                            #endregion
                            #region Pet 31

                            case InteractionType.pet31:
                                var PetData31 = ExtraData.Split('\n');
                                var GeneratedPet31 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData31[0], 31,
                                    PetData31[1], PetData31[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet31);
                                break;

                            #endregion
                            #region Pet 32

                            case InteractionType.pet32:
                                var PetData32 = ExtraData.Split('\n');
                                var GeneratedPet32 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData32[0], 32,
                                    PetData32[1], PetData32[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet32);
                                break;

                            #endregion
                            #region Pet 33

                            case InteractionType.pet33:
                                var PetData33 = ExtraData.Split('\n');
                                var GeneratedPet33 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData33[0], 33,
                                    PetData33[1], PetData33[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet33);
                                break;

                            #endregion
                            #region Pet 34

                            case InteractionType.pet34:
                                var PetData34 = ExtraData.Split('\n');
                                var GeneratedPet34 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData34[0], 34,
                                    PetData34[1], PetData34[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet34);
                                break;

                            #endregion
                            #region Pet 35

                            case InteractionType.pet35:
                                var PetData35 = ExtraData.Split('\n');
                                var GeneratedPet35 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData35[0], 35,
                                    PetData35[1], PetData35[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet35);
                                break;

                            #endregion
                            #region Pet 36

                            case InteractionType.pet36:
                                var PetData36 = ExtraData.Split('\n');
                                var GeneratedPet36 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData36[0], 36,
                                    PetData36[1], PetData36[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet36);
                                break;

                            #endregion
                            #region Pet 37

                            case InteractionType.pet37:
                                var PetData37 = ExtraData.Split('\n');
                                var GeneratedPet37 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData37[0], 37,
                                    PetData37[1], PetData37[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet37);
                                break;

                            #endregion
                            #region Pet 38

                            case InteractionType.pet38:
                                var PetData38 = ExtraData.Split('\n');
                                var GeneratedPet38 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData38[0], 38,
                                    PetData38[1], PetData38[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet38);
                                break;

                            #endregion
                            #region Pet 39

                            case InteractionType.pet39:
                                var PetData39 = ExtraData.Split('\n');
                                var GeneratedPet39 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData39[0], 39,
                                    PetData39[1], PetData39[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet39);
                                break;

                            #endregion
                            #region Pet 40

                            case InteractionType.pet40:
                                var PetData40 = ExtraData.Split('\n');
                                var GeneratedPet40 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData40[0], 40,
                                    PetData40[1], PetData40[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet40);
                                break;

                            #endregion
                            #region Pet 41

                            case InteractionType.pet41:
                                var PetData41 = ExtraData.Split('\n');
                                var GeneratedPet41 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData41[0], 41,
                                    PetData41[1], PetData41[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet41);
                                break;

                            #endregion
                            #region Pet 42

                            case InteractionType.pet42:
                                var PetData42 = ExtraData.Split('\n');
                                var GeneratedPet42 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData42[0], 42,
                                    PetData42[1], PetData42[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet42);
                                break;

                            #endregion
                            #region Pet 43

                            case InteractionType.pet43:
                                var PetData43 = ExtraData.Split('\n');
                                var GeneratedPet43 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData43[0], 43,
                                    PetData43[1], PetData43[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet43);
                                break;

                            #endregion
                            #region Pet 44

                            case InteractionType.pet44:
                                var PetData44 = ExtraData.Split('\n');
                                var GeneratedPet44 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData44[0], 44,
                                    PetData44[1], PetData44[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet44);
                                break;

                            #endregion
                            #region Pet 45

                            case InteractionType.pet45:
                                var PetData45 = ExtraData.Split('\n');
                                var GeneratedPet45 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData45[0], 45,
                                    PetData45[1], PetData45[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet45);
                                break;

                            #endregion
                            #region Pet 46

                            case InteractionType.pet46:
                                var PetData46 = ExtraData.Split('\n');
                                var GeneratedPet46 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData46[0], 46,
                                    PetData46[1], PetData46[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet46);
                                break;

                            #endregion
                            #region Pet 47

                            case InteractionType.pet47:
                                var PetData47 = ExtraData.Split('\n');
                                var GeneratedPet47 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData47[0], 47,
                                    PetData47[1], PetData47[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet47);
                                break;

                            #endregion
                            #region Pet 48

                            case InteractionType.pet48:
                                var PetData48 = ExtraData.Split('\n');
                                var GeneratedPet48 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData48[0], 48,
                                    PetData48[1], PetData48[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet48);
                                break;

                            #endregion
                            #region Pet 49

                            case InteractionType.pet49:
                                var PetData49 = ExtraData.Split('\n');
                                var GeneratedPet49 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData49[0], 49,
                                    PetData49[1], PetData49[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet49);
                                break;

                            #endregion
                            #region Pet 50

                            case InteractionType.pet50:
                                var PetData50 = ExtraData.Split('\n');
                                var GeneratedPet50 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData50[0], 50,
                                    PetData50[1], PetData50[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet50);
                                break;

                            #endregion
                            #region Pet 51

                            case InteractionType.pet51:
                                var PetData51 = ExtraData.Split('\n');
                                var GeneratedPet51 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData51[0], 51,
                                    PetData51[1], PetData51[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet51);
                                break;

                            #endregion
                            #region Pet 52

                            case InteractionType.pet52:
                                var PetData52 = ExtraData.Split('\n');
                                var GeneratedPet52 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData52[0], 52,
                                    PetData52[1], PetData52[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet52);
                                break;

                            #endregion
                            #region Pet 53

                            case InteractionType.pet53:
                                var PetData53 = ExtraData.Split('\n');
                                var GeneratedPet53 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData53[0], 53,
                                    PetData53[1], PetData53[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet53);
                                break;

                            #endregion
                            #region Pet 54

                            case InteractionType.pet54:
                                var PetData54 = ExtraData.Split('\n');
                                var GeneratedPet54 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData54[0], 54,
                                    PetData54[1], PetData54[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet54);
                                break;

                            #endregion
                            #region Pet 55

                            case InteractionType.pet55:
                                var PetData55 = ExtraData.Split('\n');
                                var GeneratedPet55 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData55[0], 55,
                                    PetData55[1], PetData55[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet55);
                                break;

                            #endregion
                            #region Pet 56

                            case InteractionType.pet56:
                                var PetData56 = ExtraData.Split('\n');
                                var GeneratedPet56 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData56[0], 56,
                                    PetData56[1], PetData56[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet56);
                                break;

                            #endregion
                            #region Pet 57

                            case InteractionType.pet57:
                                var PetData57 = ExtraData.Split('\n');
                                var GeneratedPet57 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData57[0], 57,
                                    PetData57[1], PetData57[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet57);
                                break;

                            #endregion
                            #region Pet 58

                            case InteractionType.pet58:
                                var PetData58 = ExtraData.Split('\n');
                                var GeneratedPet58 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData58[0], 58,
                                    PetData58[1], PetData58[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet58);
                                break;

                            #endregion
                            #region Pet 59

                            case InteractionType.pet59:
                                var PetData59 = ExtraData.Split('\n');
                                var GeneratedPet59 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData59[0], 59,
                                    PetData59[1], PetData59[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet59);
                                break;

                            #endregion
                            #region Pet 60

                            case InteractionType.pet60:
                                var PetData60 = ExtraData.Split('\n');
                                var GeneratedPet60 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData60[0], 60,
                                    PetData60[1], PetData60[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet60);
                                break;

                            #endregion
                            #region Pet 61

                            case InteractionType.pet61:
                                var PetData61 = ExtraData.Split('\n');
                                var GeneratedPet61 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData61[0], 61,
                                    PetData61[1], PetData61[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet61);
                                break;

                            #endregion
                            #region Pet 62

                            case InteractionType.pet62:
                                var PetData62 = ExtraData.Split('\n');
                                var GeneratedPet62 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData62[0], 62,
                                    PetData62[1], PetData62[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet62);
                                break;

                            #endregion
                            #region Pet 63

                            case InteractionType.pet63:
                                var PetData63 = ExtraData.Split('\n');
                                var GeneratedPet63 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData63[0], 63,
                                    PetData63[1], PetData63[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet63);
                                break;

                            #endregion
                            #region Pet 64

                            case InteractionType.pet64:
                                var PetData64 = ExtraData.Split('\n');
                                var GeneratedPet64 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData64[0], 64,
                                    PetData64[1], PetData64[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet64);
                                break;

                            #endregion
                            #region Pet 65

                            case InteractionType.pet65:
                                var PetData65 = ExtraData.Split('\n');
                                var GeneratedPet65 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData65[0], 65,
                                    PetData65[1], PetData65[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet65);
                                break;

                            #endregion
                            #region Pet 66

                            case InteractionType.pet66:
                                var PetData66 = ExtraData.Split('\n');
                                var GeneratedPet66 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData66[0], 66,
                                    PetData66[1], PetData66[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet66);
                                break;

                            #endregion
                            #region Pet 67

                            case InteractionType.pet67:
                                var PetData67 = ExtraData.Split('\n');
                                var GeneratedPet67 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData67[0], 67,
                                    PetData67[1], PetData67[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet67);
                                break;

                            #endregion
                            #region Pet 68

                            case InteractionType.pet68:
                                var PetData68 = ExtraData.Split('\n');
                                var GeneratedPet68 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData68[0], 68,
                                    PetData68[1], PetData68[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet68);
                                break;

                            #endregion
                            #region Pet 69

                            case InteractionType.pet69:
                                var PetData69 = ExtraData.Split('\n');
                                var GeneratedPet69 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData69[0], 69,
                                    PetData69[1], PetData69[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet69);
                                break;

                            #endregion
                            #region Pet 70

                            case InteractionType.pet70:
                                var PetData70 = ExtraData.Split('\n');
                                var GeneratedPet70 = PetUtility.CreatePet(Session.GetHabbo().Id, PetData70[0], 70,
                                    PetData70[1], PetData70[2]);

                                Session.GetHabbo().GetInventoryComponent().TryAddPet(GeneratedPet70);
                                break;

                                #endregion
                                #endregion
                        }

                        Session.SendMessage(new FurniListNotificationComposer(0, 3));
                        Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

                        ItemData PetFood = null;
                        if (QuasarEnvironment.GetGame().GetItemManager().GetItem(320, out PetFood))
                        {
                            Item Food = ItemFactory.CreateSingleItemNullable(PetFood, Session.GetHabbo(), "", "");
                            if (Food != null)
                            {
                                Session.GetHabbo().GetInventoryComponent().TryAddItem(Food);
                                Session.SendMessage(new FurniListNotificationComposer(Food.Id, 1));
                            }
                        }
                        break;
                    }
            }

            if (Item.Badge != string.Empty) Session.GetHabbo().GetBadgeComponent().GiveBadge(Item.Badge, true, Session);
            Session.SendMessage(new PurchaseOKComposer(Item, Item.Data));
            Session.SendMessage(new FurniListUpdateComposer());
        }
    }
}