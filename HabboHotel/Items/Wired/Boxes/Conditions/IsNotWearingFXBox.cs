﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Conditions
{
    class IsNotWearingFXBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.ConditionIsWearingFX; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public IsNotWearingFXBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int Unknown2 = Packet.PopInt();

            this.StringData = Unknown2.ToString();
        }

        public bool Execute(params object[] Params)
        {
            if (Params.Length == 0)
                return false;

            if (String.IsNullOrEmpty(this.StringData))
                return false;

            Habbo Player = (Habbo)Params[0];
            if (Player == null)
                return false;

            if (Player.Effects().CurrentEffect != int.Parse(this.StringData))
                return true;
            return false;
        }
    }
}
