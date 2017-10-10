using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (QuasarEnvironment.GetDBConfig().DBData["exchange_enabled"] != "1")
            {
                Session.SendNotification("Het inwisselen van wisselkoers is op dit moment uitgezet door het Hotel Management.");
                return;
            }

            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
                return;

            if (Exchange.GetBaseItem().ItemName.StartsWith("CFC_"))
            {

                string[] Split = Exchange.GetBaseItem().ItemName.Split('_');
                int Value = int.Parse(Split[1]);

                if (Value > 0)
                {
                    Session.GetHabbo().Credits += Value;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }
            }

            if (Exchange.GetBaseItem().ItemName.StartsWith("CF_"))
            {

                string[] Split = Exchange.GetBaseItem().ItemName.Split('_');
                int Value = int.Parse(Split[1]);

                if (Value > 0)
                {
                    Session.GetHabbo().Credits += Value;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }
            }

            // Added
            if (Exchange.GetBaseItem().ItemName.StartsWith("DCK_"))
            {

                string[] Split = Exchange.GetBaseItem().ItemName.Split('_');
                int Value = int.Parse(Split[1]);

                if (Value > 0)
                {
                    
                    Session.GetHabbo().Duckets += Value;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Value));
                }
            }

            if (Exchange.GetBaseItem().ItemName.StartsWith("DIA_"))
            {

                string[] Split = Exchange.GetBaseItem().ItemName.Split('_');
                int Value = int.Parse(Split[1]);

                if (Value > 0)
                {

                    Session.GetHabbo().Diamonds += Value;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Value, 5));
                }
            }

            // Until here
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Exchange.Id + "' LIMIT 1");
            }

            Session.SendMessage(new FurniListUpdateComposer());
            Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id, false);
            Session.GetHabbo().GetInventoryComponent().RemoveItem(Exchange.Id);

        }
    }
}
