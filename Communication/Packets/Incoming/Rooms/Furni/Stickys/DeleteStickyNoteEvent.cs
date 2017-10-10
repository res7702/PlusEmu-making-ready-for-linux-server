using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni.Stickys
{
    class DeleteStickyNoteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session))
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Item == null)
                return;

            if (Item.GetBaseItem().InteractionType == InteractionType.POSTIT || Item.GetBaseItem().InteractionType == InteractionType.CAMERA_PICTURE)
            {
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                }
            }
        }
    }
}
