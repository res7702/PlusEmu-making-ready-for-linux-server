using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Communication.Packets.Outgoing.Rooms.Furni;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class ExecuteCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int craftingTable = Packet.PopInt();
            string recipeName = Packet.PopString();

            //Console.WriteLine("ID Mesa: " + craftingTable);
            //Console.WriteLine("Receta: " + recipeName);

            var recipe = QuasarEnvironment.GetGame().GetCraftingManager().GetRecipe(recipeName);

            if (recipe == null) return;
            ItemData resultItem = QuasarEnvironment.GetGame().GetItemManager().GetItemByName(recipe.Result);
            if (resultItem == null) return;
            bool success = true;
            foreach (var need in recipe.ItemsNeeded)
            {
                for (var i = 1; i <= need.Value; i++)
                {
                    ItemData item = QuasarEnvironment.GetGame().GetItemManager().GetItemByName(need.Key);
                    if (item == null)
                    {
                        success = false;
                        continue;
                    }
                    var inv = Session.GetHabbo().GetInventoryComponent().GetFirstItemByBaseId(item.Id);
                    if (inv == null)
                    {
                        success = false;
                        continue;
                    }
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(inv.Id);
                }
            }
            if (success)
            {
                Session.GetHabbo().GetInventoryComponent().AddNewItem(0, resultItem.Id, "", 0, true, false, 0, 0);
                Session.SendMessage(new FurniListUpdateComposer());
            }
            Session.SendMessage(new CraftingResultComposer(recipe, success));
        }
    }
}