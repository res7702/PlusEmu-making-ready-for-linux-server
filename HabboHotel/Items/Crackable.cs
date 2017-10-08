using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Threading.Tasks;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Furni;

namespace Quasar.HabboHotel.Items
{
    internal class CrackableItem
    {
        internal UInt32 ItemId;
        internal List<CrackableRewards> Rewards;

        internal CrackableItem(DataRow dRow)
        {
            ItemId = Convert.ToUInt32(dRow["item_baseid"]);
            var rewardsString = (string)dRow["rewards"];

            Rewards = new List<CrackableRewards>();
            foreach (var reward in rewardsString.Split(';'))
            {
                var rewardType = reward.Split(',')[0];
                var rewardItem = reward.Split(',')[1];
                var rewardLevel = uint.Parse(reward.Split(',')[2]);
                Rewards.Add(new CrackableRewards(ItemId, rewardType, rewardItem, rewardLevel));
            }
        }
    }

    internal class CrackableRewards
    {
        internal UInt32 CrackableId, CrackableLevel;
        internal String CrackableRewardType, CrackableReward;

        internal CrackableRewards(uint crackableId, string crackableRewardType, string crackableReward, uint crackableLevel)
        {
            CrackableId = crackableId;
            CrackableRewardType = crackableRewardType;
            CrackableReward = crackableReward;
            CrackableLevel = crackableLevel;
        }
    }

    internal class CrackableManager
    {
        internal Dictionary<Int32, CrackableItem> Crackable;

        internal void Initialize(IQueryAdapter dbClient)
        {
            Crackable = new Dictionary<Int32, CrackableItem>();
            dbClient.SetQuery("SELECT * FROM crackable_rewards");
            var table = dbClient.getTable();
            foreach (DataRow dRow in table.Rows)
            {
                if (Crackable.ContainsKey(Convert.ToInt32(dRow["item_baseid"]))) continue;
                Crackable.Add(Convert.ToInt32(dRow["item_baseid"]), new CrackableItem(dRow));
            }
        }


        private List<CrackableRewards> GetRewardsByLevel(int itemId, int level)
        {
            var rewards = new List<CrackableRewards>();
            foreach (var reward in Crackable[itemId].Rewards.Where(furni => furni.CrackableLevel == level)) rewards.Add(reward);
            return rewards;
        }

        internal void ReceiveCrackableReward(RoomUser user, Room room, Item item)
        {
            if (room == null || item == null) return;
            if (item.GetBaseItem().InteractionType != InteractionType.PINATA && item.GetBaseItem().InteractionType != InteractionType.MAGICEGG) return;
            if (!Crackable.ContainsKey(item.GetBaseItem().Id)) return;
            CrackableItem crackable;
            Crackable.TryGetValue(item.GetBaseItem().Id, out crackable);
            if (crackable == null) return;
            int x = item.GetX, y = item.GetY;
            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
            var level = 0;
            var rand = new Random().Next(0, 200);
            if (rand == 0) level = 25;                      // 0.5%
            else if (rand == 100) level = 24;               // 0.5%
            else if (rand == 200) level = 23;               // 0.5%
                                                            // 1,5% [Eind deel 1]
            else if (rand >= 5 && rand < 10) level = 22;    // 2,5%  
            else if (rand >= 10 && rand < 15) level = 21;   // 2,5% 
            else if (rand >= 15 && rand < 20) level = 20;   // 2,5%
            else if (rand >= 20 && rand < 25) level = 19;   // 2,5%
            else if (rand >= 25 && rand < 30) level = 18;   // 2.5%
                                                            // 14% [Eind deel 2]
            else if (rand >= 30 && rand < 40) level = 17;   // 5%
            else if (rand >= 40 && rand < 50) level = 16;   // 5%
            else if (rand >= 50 && rand < 60) level = 15;   // 5%
            else if (rand >= 60 && rand < 70) level = 14;   // 5%
            else if (rand >= 70 && rand < 80) level = 13;   // 5%
            else if (rand >= 80 && rand < 90) level = 12;   // 5&
                                                            // 34% [Eind deel 3]
            else if (rand >= 90 && rand < 95) level = 11;   // 2.5%
            else if (rand >= 95 && rand < 100) level = 10;  // 2.5%

            else if (rand >= 161 && rand < 164) level = 9;  // 1,5%
            else if (rand >= 164 && rand < 167) level = 8;  // 1,5%
            else if (rand >= 167 && rand < 170) level = 7;  // 1,5%
            else if (rand >= 170 && rand < 173) level = 6;  // 1,5%
                                                            
            else if (rand >= 173 && rand < 183) level = 5;  // 5%
            else if (rand >= 183 && rand < 193) level = 4;  // 5%
            else if (rand >= 193 && rand < 195) level = 3;  // 1%
            else if (rand >= 195 && rand < 200) level = 2;  // 2.5%
            else if (rand >= 101 && rand < 161) level = 1;  // 30%
            //                                              // == 100%
            else
            {
                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "!", ""));
                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_error", "Potver! Je hebt niks gewonnen, de volgende keer beter.", ""));
                return;
            }

            var possibleRewards = GetRewardsByLevel((int)crackable.ItemId, level);
            var reward = possibleRewards[new Random().Next(0, (possibleRewards.Count - 1))];

            Task.Run(() =>
            {
            using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {

                #region Prijzen
                switch (reward.CrackableRewardType)
                {
                        #region Meubi
                    case "item":
                        
                        goto ItemType;
                    #endregion
    
                        #region Fail
                        case "fail":
                            user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "!", ""));
                            user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_error", "Potver! Je hebt niks gewonnen, de volgende keer beter.","" ));
                            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                            dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                            return;
                        #endregion

                        #region Credits
                        case "credits":
                            {
                                user.GetClient().GetHabbo().Credits += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new CreditBalanceComposer(user.GetClient().GetHabbo().Credits));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Gefeliteerd met je prijs!", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("cred", "Woehoe, prijs! Je hebt " + int.Parse(reward.CrackableReward) + " Credits gekregen!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region Duckets
                        case "duckets":
                            {
                                user.GetClient().GetHabbo().Duckets += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Duckets, user.GetClient().GetHabbo().Duckets));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Gefeliteerd met je prijs!", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("duckets", "Woehoe, prijs! Je hebt " + int.Parse(reward.CrackableReward) + " Duckets gekregen!.", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region Diamanten
                        case "diamonds":
                            {
                                user.GetClient().GetHabbo().Diamonds += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Diamonds, 0, 5));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Gefeliteerd met je prijs!", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("diamonds", "Woehoe, prijs! Je hebt " + int.Parse(reward.CrackableReward) + " Diamanten gekregen!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region Honor
                        case "honors":
                            {
                                user.GetClient().GetHabbo().GOTWPoints += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().GOTWPoints, 0, 103));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Gefeliteerd met je prijs!", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("honor", "Woehoe, prijs! Je hebt " + int.Parse(reward.CrackableReward) + " * gekregen!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region Badges
                        case "badge":
                            {
                                if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(reward.CrackableReward))
                                {
                                    user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Aah verloren!", ""));
                                    user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("error", "Test"));
                                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                    dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                    return;
                                }

                                user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(reward.CrackableReward, true, user.GetClient());

                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "! Gefeliteerd met je prijs!", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("badge", "Woehoe, prijs! Je hebt de badge " + (reward.CrackableReward) + " gekregen!", ""));
                                
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                               // QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(user.GetClient(), "ACH_Login", 1);

                                return;
                            }
                            #endregion
                    }
                #endregion

                ItemType:
                    user.GetClient().SendMessage(new OpenGiftComposer(item.Data, item.ExtraData, item, true)); // custom tocan2
                    item.MagicRemove = true; // custom tocan2
                    room.SendMessage(new ObjectUpdateComposer(item, Convert.ToInt32(user.GetClient().GetHabbo().Id))); //custom tocan2
                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                    dbClient.runFastQuery("UPDATE items SET base_item = " + int.Parse(reward.CrackableReward) + ", extra_data = '' WHERE id = " + item.Id);
                    item.BaseItem = int.Parse(reward.CrackableReward);
                    item.ResetBaseItem();
                    item.ExtraData = string.Empty;
                    if (!room.GetRoomItemHandler().SetFloorItem(user.GetClient(), item, item.GetX, item.GetY, item.Rotation, true, false, true))
                    {
                        dbClient.runFastQuery("UPDATE items SET room_id = 0 WHERE id = " + item.Id);
                        user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
                    }
                }


                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_gewonnen", "Je hebt het nummer " + rand + "!", ""));
                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("item", "Woehoe, prijs! Je hebt een zeldzaam meubi gekregen!", ""));
            });
        }

    }
}