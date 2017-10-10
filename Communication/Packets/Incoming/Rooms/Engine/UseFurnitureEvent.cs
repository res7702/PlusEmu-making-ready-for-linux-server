using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Items.Wired;
using Quasar.Communication.Packets.Outgoing.Rooms.Furni;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

using Quasar.Database.Interfaces;



namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class UseFurnitureEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int itemID = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(itemID);
            var User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Item == null)
                return;

            if (User != null && User.ClickRotate)
            {
                var Rotation = Item.Rotation + 2;
                if (Rotation > 6) Rotation = 0;

                if (Room.GetRoomItemHandler().SetFloorItem(Session, Item, Item.GetX, Item.GetY, Rotation, false, false, true)) return;
                Room.SendMessage(new ObjectUpdateComposer(Item, Room.OwnerId));
                return;
            }

            bool hasRights = false;
            if (Room.CheckRights(Session, false, true))
                hasRights = true;

            if (Item.GetBaseItem().InteractionType == InteractionType.banzaitele)
                return;

            if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
            {
                if (!Room.CheckRights(Session, true))
                    return;
                if (Room.TonerData.Enabled == 0)
                    Room.TonerData.Enabled = 1;
                else
                    Room.TonerData.Enabled = 0;

                Room.SendMessage(new ObjectUpdateComposer(Item, Room.OwnerId));

                Item.UpdateState();

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `room_items_toner` SET `enabled` = '" + Room.TonerData.Enabled + "' LIMIT 1");
                }
                return;
            }

            if (Item.Data.InteractionType == InteractionType.GNOME_BOX && Item.UserID == Session.GetHabbo().Id)
            {
                Session.SendMessage(new GnomeBoxComposer(Item.Id));
            }

            Boolean Toggle = true;
            if (Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_1 || Item.GetBaseItem().InteractionType == InteractionType.WF_FLOOR_SWITCH_2)
            {
                RoomUser User1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User1 == null)
                    return;

                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_WiredExpertUseHandle", 1);

                if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
                {
                    Toggle = false;
                }
            }

                string oldData = Item.ExtraData;
                int request = Packet.PopInt();

                Item.Interactor.OnTrigger(Session, Item, request, hasRights);

                if (Toggle)
                    Item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, Session.GetHabbo(), Item);

                QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.EXPLORE_FIND_ITEM, Item.GetBaseItem().Id);
        }
    }
}
