using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;


using Quasar.HabboHotel.Rooms.Trading;
using Quasar.HabboHotel.Items;


namespace Quasar.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer(Trade Trade)
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {
            if (Trade.Users.Count() < 2)
                return;

            var User1 = Trade.Users.First();
            var User2 = Trade.Users.Last();

            base.WriteInteger(User1.GetClient().GetHabbo().Id);
            SerializeUserItems(User1);

            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);

            SerializeUserItems(User2);

            base.WriteInteger(0);
            base.WriteInteger(0);

        }
        private void SerializeUserItems(TradeUser User)
        {
            base.WriteInteger(User.OfferedItems.Count);//While
            foreach (Item Item in User.OfferedItems.ToList())
            {
                base.WriteInteger(Item.Id);
                base.WriteString(Item.Data.Type.ToString().ToUpper());
                base.WriteInteger(Item.Id);
                base.WriteInteger(Item.Data.SpriteId);
                base.WriteInteger(1);
                base.WriteBoolean(true);


                //Func called _SafeStr_15990
                base.WriteInteger(0);
                base.WriteString("");


                //end Func called
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteInteger(0);
                if (Item.Data.Type.ToString().ToUpper() == "S")
                    base.WriteInteger(0);
            }
            //End of while
        }
    }
}