#region

using System;
using System.Data;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class WisselkoersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_wisselkoers"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Wissel je wisselkoers credits om naar je portemonnee."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var TotalValue = 0;

            try
            {
                DataTable Table = null;
                using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id +
                                      "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.getTable();
                }

                if (Table == null)
                {
                    Session.SendWhisper("Je hebt geen wisselkoers items in je inventaris!");
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    var Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                        continue;

                    if (!Item.GetBaseItem().ItemName.StartsWith("CF_") &&
                        !Item.GetBaseItem().ItemName.StartsWith("CFC_"))
                        continue;

                    if (Item.RoomId > 0)
                        continue;

                    var Split = Item.GetBaseItem().ItemName.Split('_');
                    var Value = int.Parse(Split[1]);

                    using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                    TotalValue += Value;

                    if (Value > 0)
                    {
                        Session.GetHabbo().Credits += Value;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }
                }

                if (TotalValue > 0)
                    Session.SendNotification("Al je wisselkoers items zijn nu in je portemonnee\r\r(Totale waarde: " +
                                             TotalValue + " credits!");
                else
                    Session.SendNotification("Je hebt geen wisselkoers items in je inventaris!");
            }
            catch
            {
                Session.SendNotification("Oeps, er is iets mis gegaan bij het omwisselen. Meld het bij een Staff-lid.");
            }
        }
    }
}