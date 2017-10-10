using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Catalog.Clothing;



using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Quasar.Database.Interfaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data == null)
                return;

            if (Item.UserID != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                Session.SendNotification("Oeps! Dit kledingstuk is niet te koop.");
                return;
            }

            if (Item.Data.ClothingId == 0)
            {
                Session.SendNotification("Oeps! Dit item is niet ingesteld als een kledingstuk.");
                return;
            }

            ClothingItem Clothing = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(Item.Data.ClothingId, out Clothing))
            {
                Session.SendNotification("Oeps! Dit kledingstuk kan helaas niet worden gevonden.");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            Session.GetHabbo().GetClothing().AddClothing(Clothing.ClothingName, Clothing.PartIds);
            Session.SendMessage(new FigureSetIdsComposer(Session.GetHabbo().GetClothing().GetClothingAllParts));
            Session.SendMessage(new RoomNotificationComposer("figureset.redeemed.success"));
            Session.SendWhisper("Wanneer het kledij niet volledig correct zichbaar is kan het helpen om het hotel even te herladen!");
        }
    }
}
