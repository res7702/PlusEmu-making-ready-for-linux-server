﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.HabboHotel.Items.Crafting;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Furni
{
    class CraftingFoundComposer : ServerPacket
    {
        public CraftingFoundComposer(int count, bool found)
            : base(ServerPacketHeader.CraftingFoundMessageComposer) //resultado
        {
            base.WriteInteger(count); //hay mas?
            base.WriteBoolean(found); //encontrado
        }
    }
}