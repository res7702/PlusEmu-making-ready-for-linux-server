using Quasar.Communication.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Inventory.Purse
{
    class GetHabboClubCenterInfoMessageComposer : ServerPacket
    {
        public GetHabboClubCenterInfoMessageComposer() : base(ServerPacketHeader.HabboClubCenterInfoMessageComposer)
        {
            base.WriteInteger(2005);//streakduration in days 
            base.WriteString("Permanent");//joindate 
            base.WriteInteger(0); base.WriteInteger(0);//this should be a double 
            base.WriteInteger(0);//unused 
            base.WriteInteger(0);//unused 
            base.WriteInteger(10);//spentcredits 
            base.WriteInteger(20);//streakbonus 
            base.WriteInteger(10);//spentcredits 
            base.WriteInteger(0);//next pay in minutes
        }
    }
}