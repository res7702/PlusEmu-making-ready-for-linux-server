﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Catalog;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveRewardBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.EffectGiveReward; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GiveRewardBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Often = Packet.PopInt();
            bool Unique = (Packet.PopInt() == 1);
            int Limit = Packet.PopInt();
            int Often_No = Packet.PopInt();
            string Reward = Packet.PopString();

            this.BoolData = Unique;
            this.StringData = Reward + "-" + Often + "-" + Limit;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Owner = QuasarEnvironment.GetHabboById(Item.UserID);
            if (Owner == null || !Owner.GetPermissions().HasRight("room_item_wired_rewards"))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            int amountLeft = int.Parse(this.StringData.Split('-')[2]);
            int often = int.Parse(this.StringData.Split('-')[1]);
            bool unique = this.BoolData;

            bool premied = false;

            if (amountLeft == 1)
            {
                Player.GetClient().SendNotification("Ya no hay mas premios, vuelve mas tarde.");
                return true;
            }

            foreach (var dataStr in (this.StringData.Split('-')[0]).Split(';'))
            {
                var dataArray = dataStr.Split(',');

                var isbadge = dataArray[0] == "0";
                var code = dataArray[1];
                var percentage = int.Parse(dataArray[2]);

                var random = QuasarEnvironment.GetRandomNumber(0, 100);

                if (!unique && percentage < random)
                    continue;
                premied = true;

                if (isbadge)
                {
                    if (Player.GetBadgeComponent().HasBadge(code))
                        Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Je hebt een Badge ontvangen die je al eerder in je inventaris had staan!", 0, User.LastBubble));
                    else
                    {
                        Player.GetBadgeComponent().GiveBadge(code, true, Player.GetClient());
                        Player.GetClient().SendNotification("Je hebt een Badge ontvangen!");
                    }
                }
                else
                {
                    ItemData ItemData = null;

                    if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(int.Parse(code), out ItemData))
                    {
                        Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "Kon item niet laden: " + code, 0, User.LastBubble));
                        return false;
                    }

                    Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Player.GetClient().GetHabbo(), "", "", 0, 0, 0);


                    if (Item != null)
                    {
                        Player.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(Item);
                        Player.GetClient().SendMessage(new FurniListNotificationComposer(Item.Id, 1));
                        Player.GetClient().SendMessage(new PurchaseOKComposer());
                        Player.GetClient().SendMessage(new FurniListAddComposer(Item));
                        Player.GetClient().SendMessage(new FurniListUpdateComposer());
                        Player.GetClient().SendNotification("¡Has recibido un regalo! Revisa tu inventario.");
                    }
                }
            }

            if (!premied)
            {
                Player.GetClient().SendNotification("Mweh, volgende keer weer een kans.");
            }
            else if (amountLeft > 1)
            {
                amountLeft--;
                this.StringData.Split('-')[2] = amountLeft.ToString();
            }

            return true;
        }
    }
}