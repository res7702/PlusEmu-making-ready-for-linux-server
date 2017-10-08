using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Database.Interfaces;
using System.Data;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class UpdateMeubiCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_updatemeubi"; }
        }

        public string Parameters
        {
            get { return ""; }
        }


        public string Description
        {
            get { return "Verander een meubi."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser RUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            List<Item> Items = Room.GetGameMap().GetRoomItemForSquare(RUser.X, RUser.Y);
            if (Params.Length == 1 || Params[1] == "help")
            {
                Session.SendMessage(new MassEventComposer("habbopages/updatemeubi"));
                return;
            }
            String Type = Params[1].ToLower();
            int numeroint = 0, FurnitureID = 0;
            double numerodouble = 0;
            DataRow Item = null;
            String opcion = "";
            switch (Type)
            {
                case "width":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `width` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item width: " + FurnitureID + " geüpdatet (Nu is de width: " + numeroint.ToString() + ")");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Alleen geldige nummers)");
                        }
                    }
                    break;

                case "length":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `length` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item length: " + FurnitureID + " geüpdatet (Nu is de length: " + numeroint.ToString() + ")");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Alleen geldige nummers)");
                        }
                    }
                    break;

                case "height":
                    {
                        try
                        {
                            numerodouble = Convert.ToDouble(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `stack_height` = '" + numerodouble + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item height: " + FurnitureID + " geüpdatet (Nu is de height: " + numerodouble.ToString() + ")");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Alleen geldige nummers)");
                        }
                    }
                    break;

                case "interactioncount":
                    {
                        try
                        {
                            numeroint = Convert.ToInt32(Params[2]);
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_modes_count` = '" + numeroint + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Interaction count: " + FurnitureID + " geüpdatet (Nu is het mogelijk om het meubi " + numeroint.ToString() + " te veranderen.)");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Alleen geldige nummers)");
                        }
                    }
                    break;

                case "cansit":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("ja") && !opcion.Equals("nee"))
                            {
                                Session.SendWhisper("Gebruik enkel 'ja' of 'nee'.");
                                return;
                            }
                            if (opcion.Equals("ja"))
                                opcion = "1";
                            else if (opcion.Equals("nee"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_sit` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item can_sit: " + FurnitureID + " geüpdatet.");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Gebruik enkel 'ja' of 'nee')");
                        }
                    }
                    break;

                case "canstack":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("ja") && !opcion.Equals("nee"))
                            {
                                Session.SendWhisper("Gebruik enkel 'ja' of 'nee'.");
                                return;
                            }
                            if (opcion.Equals("ja"))
                                opcion = "1";
                            else if (opcion.Equals("nee"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `can_stack` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item can_stack: " + FurnitureID + " geüpdatet");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Gebruik enkel 'ja' of 'nee'.");
                        }
                    }
                    break;

                case "canwalk":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("ja") && !opcion.Equals("nee"))
                            {
                                Session.SendWhisper("Gebruik enkel 'ja' of 'nee'.");
                                return;
                            }
                            if (opcion.Equals("ja"))
                                opcion = "1";
                            else if (opcion.Equals("nee"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_walkable` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item can_walk: " + FurnitureID + " geüpdatet.");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Gebruik enkel 'ja' of 'nee'.)");
                        }
                    }
                    break;

                case "marktplaats":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            if (!opcion.Equals("ja") && !opcion.Equals("nee"))
                            {
                                Session.SendWhisper("Gebruik enkel 'ja' of 'nee'.");
                                return;
                            }
                            if (opcion.Equals("ja"))
                                opcion = "1";
                            else if (opcion.Equals("nee"))
                                opcion = "0";
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `is_rare` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Marktplaats optie: " + FurnitureID + " geüpdatet.");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Gebruik enkel 'ja' of 'nee'.)");
                        }
                    }
                    break;

                case "interaction":
                    {
                        try
                        {
                            opcion = Params[2].ToLower();
                            foreach (Item IItem in Items.ToList())
                            {
                                if (IItem == null)
                                    continue;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("SELECT base_item FROM items WHERE id = '" + IItem.Id + "' LIMIT 1");
                                    Item = dbClient.getRow();
                                    if (Item == null)
                                        continue;
                                    FurnitureID = Convert.ToInt32(Item[0]);
                                    dbClient.RunQuery("UPDATE `furniture` SET `interaction_type` = '" + opcion + "' WHERE `id` = '" + FurnitureID + "' LIMIT 1");
                                }
                                Session.SendWhisper("Item interaction: " + FurnitureID + " updated. (New interaction: " + opcion + ")");
                            }
                            QuasarEnvironment.GetGame().GetItemManager().Init();
                        }
                        catch (Exception)
                        {
                            Session.SendNotification("Oeps! Er ging was mis (Alleen geldige interaction types)");
                        }
                    }
                    break;

                default:
                    {
                        Session.SendNotification("Oeps! Er ging overduidelijk wat mis voor hulp typ de command :item help.");
                        return;
                    }
                    
            }
        }
    }
}