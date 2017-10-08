using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Effects
{
    class ShowMessageBox : IWiredItem
    {
        public Room Instance { get; set; }

        public Item Item { get; set; }

        public WiredBoxType Type { get { return WiredBoxType.EffectShowMessage; } }

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public string ItemsData { get; set; }

        public ShowMessageBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string Message = Packet.PopString();

            this.StringData = Message;
        }

        public bool Execute(params object[] Params)
        {
            if (Params == null || Params.Length == 0)
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.GetClient() == null || string.IsNullOrWhiteSpace(StringData))
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            string Message = StringData;

            if (StringData.Contains("%HABBIS%"))
                Message = Message.Replace("%HABBIS%", Player.Username);

            if (StringData.Contains("%KAMERNAAM%"))
                Message = Message.Replace("%KAMERNAAM%", Player.CurrentRoom.Name);

            if (StringData.Contains("%SETMAX%"))
                Message = Message.Replace("%SETMAX%", Player.CurrentRoom.UserCount.ToString());

            if (StringData.Contains("%ONLINE%"))
                Message = Message.Replace("%ONLINE%", QuasarEnvironment.GetGame().GetClientManager().Count.ToString());

            if (StringData.Contains("%ID%"))
                Message = Message.Replace("%ID%", Convert.ToString(Player.Id));

            if (StringData.Contains("%DUCKETS%"))
                Message = Message.Replace("%DUCKETS%", Convert.ToString(Player.Duckets));

            if (StringData.Contains("%DIAMANTEN%"))
                Message = Message.Replace("%DIAMANTEN%", Convert.ToString(Player.Diamonds));

            if (StringData.Contains("%CREDITS%"))
                Message = Message.Replace("%CREDITS%", Convert.ToString(Player.Credits));

            if (StringData.Contains("%RANK%")) // Put names not number
                Message = Message.Replace("%RANK%", Convert.ToString(Player.Rank));

            if (StringData.Contains("%STEMMEN%"))
                Message = Message.Replace("%STEMMEN%", Player.CurrentRoom.Score.ToString());

            Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, 34));
            return true;
        }
    }
}