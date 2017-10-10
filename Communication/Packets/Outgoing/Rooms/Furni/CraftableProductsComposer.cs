using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Furni
{
    class CraftableProductsComposer : ServerPacket
    {
        public CraftableProductsComposer()
            : base(ServerPacketHeader.CraftableProductsMessageComposer)
        {
            var crafting = QuasarEnvironment.GetGame().GetCraftingManager();
            base.WriteInteger(crafting.CraftingRecipes.Count);
            foreach (var recipe in crafting.CraftingRecipes.Values)
            {
                base.WriteString(recipe.Result);
                base.WriteString(recipe.Result);
            }
            base.WriteInteger(crafting.CraftableItems.Count);
            foreach (var itemName in crafting.CraftableItems)
            {
                base.WriteString(itemName);
            }
        }
    }
}