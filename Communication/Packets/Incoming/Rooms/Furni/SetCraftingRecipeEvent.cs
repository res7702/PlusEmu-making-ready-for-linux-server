using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Furni;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class SetCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string recipeName = Packet.PopString();
            var recipe = QuasarEnvironment.GetGame().GetCraftingManager().GetRecipe(recipeName);
            if (recipe == null) return;
            Session.SendMessage(new CraftingRecipeComposer(recipe));
        }

    }
}