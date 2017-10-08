using System;
using System.Collections.Concurrent;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class GiveUserDanceBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectGiveUserDance; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public GiveUserDanceBox(Room Instance, Item Item)
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

            RoomUser ThisUser = User.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(User.GetClient().GetHabbo().Id);
            var dance = int.Parse(StringData);

            User.GetClient().GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, dance));
            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_wired_dans", "" + User.GetClient().GetHabbo().Username + ", is nu aan het dansen (" + StringData + ") door een Wired-effect.", ""));
            Item.UpdateState(false, true);
            Item.RequestUpdate(2, true);



            return true;
        }
    }
}