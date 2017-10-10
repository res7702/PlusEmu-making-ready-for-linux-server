using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Quests;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Items.Data;
using Quasar.HabboHotel.Items.Data.Moodlight;
using Quasar.HabboHotel.Items.Data.Toner;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class PlaceObjectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            if (Session.GetHabbo().ViewInventory)
            {
                Session.SendMessage("Oeps! Je kunt dit item niet plaatsen omdat je momenteel iemand anders zijn of haar inventaris aan het bekijken bent.");
                return;
            }

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int ItemId = 0;
            string[] Data = null;

            string RawData = Packet.PopString();
            Data = RawData.Split(' ');

            if (!int.TryParse(Data[0], out ItemId))
                return;

            bool HasRights = false;
            if (Room.CheckRights(Session, false, true))
                HasRights = true;

            if (!HasRights)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_bouwen", "Oeps! Je kan dit item hier niet plaatsen.", ""));
                return;
            }

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
                return;

            if (Room.ForSale)
            {
                Session.SendWhisper("Oeps! Je kunt de kamer niet bewerken wanneer deze te koop staat!");
                Session.SendWhisper("Als je de verkoop wilt annuleren gebruik dan de command :unload.");
                return;
            }

            if (Room.GetRoomItemHandler().GetWallAndFloor.Count() > QuasarStaticGameSettings.RoomFurnitureLimit)
            {
                Session.SendNotification("Oeps! Je kunt niet meer dan " + QuasarStaticGameSettings.RoomFurnitureLimit + " items in één kamer hebben.");
                return;
            }
            else if (Item.GetBaseItem().ItemName.ToLower().Contains("cf") && Room.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("room_item_place_exchange_anywhere"))
            {
                Session.SendNotification("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.HOLE)
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoHoleFurniCount", 1, false);


            if (Item.GetBaseItem().InteractionType == InteractionType.DICE)
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CasinoPlaceDices", 1, false);

            if (Item.GetBaseItem().InteractionType == InteractionType.POKER)
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CasinoPlayPoker", 1, false);

            //TODO: Make neat.
            switch (Item.GetBaseItem().InteractionType)
            {
                #region Interaction Types
                case InteractionType.MOODLIGHT:
                    {
                        MoodlightData moodData = Room.MoodlightData;
                        if (moodData != null && Room.GetRoomItemHandler().GetItem(moodData.ItemId) != null)
                        {
                            Session.SendMessage(new RoomCustomizedAlertComposer("Oeps! Je kunt maar één sfeerverlichter per kamer."));
                            return;
                        }
                        break;
                    }
                case InteractionType.TONER:
                    {
                        TonerData tonerData = Room.TonerData;
                        if (tonerData != null && Room.GetRoomItemHandler().GetItem(tonerData.ItemId) != null)
                        {
                            Session.SendNotification("Oeps! Je kunt maar één kamer achtergrond toner per kamer.");
                            return;
                        }
                        break;
                    }
                case InteractionType.HOPPER:
                    {
                        if (Room.GetRoomItemHandler().HopperCount > 0)
                        {
                            Session.SendNotification("Oeps! Je kunt maar één hopper per kamer.");
                            return;
                        }
                        break;
                    }
                case InteractionType.JUKEBOX:
                    {
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_JukeboxPlaceRadio", 1);
                    }
                    break;

                case InteractionType.TENT:
                case InteractionType.TENT_SMALL:
                    {
                        Room.AddTent(Item.Id);
                        break;
                    }
                    #endregion
            }
            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PLACE); // Quests
            if (!Item.IsWallItem)
            {
                if (Data.Length < 4)
                    return;

                int X = 0;
                int Y = 0;
                int Rotation = 0;

                if (!int.TryParse(Data[1], out X)) { return; }
                if (!int.TryParse(Data[2], out Y)) { return; }
                if (!int.TryParse(Data[3], out Rotation)) { return; }

                Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, X, Y, 0, Rotation, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, string.Empty, Room);
                if (Room.GetRoomItemHandler().SetFloorItem(Session, RoomItem, X, Y, Rotation, true, false, true))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

                    if (Session.GetHabbo().Id == Room.OwnerId)
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);

                    if (RoomItem.IsWired)
                    {
                        try { Room.GetWired().LoadWiredBox(RoomItem); }
                        catch { Console.WriteLine(Item.GetBaseItem().InteractionType); }
                    }

                    if (RoomItem.IsTrigger)
                    {
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_WiredExpertTriggers", 1, false);
                    }

                    if (RoomItem.IsAction)
                    {
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_WiredExpertActions", 1, false);
                    }

                    if (RoomItem.IsCondition)
                    {
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_WiredExpertConditions", 1, false);
                    }


                    if (RoomItem.IsHighscore)
                    {
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_WiredExpertHighscores", 1, false);
                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_bouwen", "Oeps! Je kan dit item hier niet plaatsen.", ""));
                    return;

                }
            }
            else if (Item.IsWallItem)
            {
                string[] CorrectedData = new string[Data.Length - 1];

                for (int i = 1; i < Data.Length; i++)
                {
                    CorrectedData[i - 1] = Data[i];
                }

                string WallPos = string.Empty;

                if (TrySetWallItem(Session.GetHabbo(), Item, CorrectedData, out WallPos))
                {
                    try
                    {
                        Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, WallPos, Room);

                        if (Room.GetRoomItemHandler().SetWallItem(Session, RoomItem))
                        {
                            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                            if (Session.GetHabbo().Id == Room.OwnerId)
                                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);
                        }
                    }
                    catch
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_bouwen", "Oeps! Je kan dit item hier niet plaatsen.", ""));
                        return;

                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_bouwen", "Oeps! Je kan dit item hier niet plaatsen.", ""));
                    return;
                }
            }
        }

        private static bool TrySetWallItem(Habbo Habbo, Item item, string[] data, out string position)
        {
            if (data.Length != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" && data[2] != "l"))
            {
                position = null;
                return false;
            }

            string wBit = data[0].Substring(3, data[0].Length - 3);
            string lBit = data[1].Substring(2, data[1].Length - 2);

            if (!wBit.Contains(",") || !lBit.Contains(","))
            {
                position = null;
                return false;
            }

            int w1 = 0;
            int w2 = 0;
            int l1 = 0;
            int l2 = 0;

            int.TryParse(wBit.Split(',')[0], out w1);
            int.TryParse(wBit.Split(',')[1], out w2);
            int.TryParse(lBit.Split(',')[0], out l1);
            int.TryParse(lBit.Split(',')[1], out l2);
            //
            //if (!Habbo.HasFuse("super_admin") && (w1 < 0 || w2 < 0 || l1 < 0 || l2 < 0 || w1 > 200 || w2 > 200 || l1 > 200 || l2 > 200))
            //{
            //    position = null;
            //    return false;
            //}



            string WallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

            position = WallPositionCheck(WallPos);

            return (position != null);
        }

        public static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                {
                    return null;
                }
                if (wallPosition.Contains(Convert.ToChar(9)))
                {
                    return null;
                }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return null;

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
                    return null;

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                if (lengthX < -1 || lengthY < -1000 || lengthX > 700 || lengthY > 700)
                    return null;

                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return null;

            }
        }
    }
}