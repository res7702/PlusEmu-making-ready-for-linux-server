using System;
using System.Collections.Generic;

using Quasar.HabboHotel.Catalog;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerRewardsComposer : ServerPacket
    {
        public RecyclerRewardsComposer()
            : base(ServerPacketHeader.RecyclerRewardsMessageComposer)
        {
            base.WriteInteger(0);// Count of items
        }
    }
}