using System;
using System.Collections.Concurrent;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserFreezeBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserFreeze; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserFreezeBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Badge = Packet.PopString();

            this.StringData = Badge;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if (String.IsNullOrEmpty(StringData))
                return false;

            var mode = int.Parse(StringData);

            if (mode == 1)
                User.Frozen = true;
            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_wired_freeze", "" + User.GetClient().GetHabbo().Username + ", is bevroren door een Wired-effect.", ""));
            if (mode == 2)
                User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_wired_freeze", "" + User.GetClient().GetHabbo().Username + ", kan weer lopen.", "")); User.Frozen = false;
                User.Frozen = false;


            return true;
        }
    }
}