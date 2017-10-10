using Quasar.HabboHotel.Calendar;
using System;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Campaigns
{
    class CalendarPrizesComposer : ServerPacket
    {
        public CalendarPrizesComposer(CalendarDay cday)
            : base(ServerPacketHeader.CalendarPrizesMessageComposer)
        {
            base.WriteBoolean(true); // enable
            base.WriteString("xmas14_starfish"); //habbo required a valid item name.. cday.ProductName); // productName: getProductData(x)
            base.WriteString(cday.ImageLink);  // customImage: //habboo-a.akamaihd.net/c_images/ + x
            base.WriteString(cday.ItemName); // getFloorItemDataByName(x)
        }
    }
}