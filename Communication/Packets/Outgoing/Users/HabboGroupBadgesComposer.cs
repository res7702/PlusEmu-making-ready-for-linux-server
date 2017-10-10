using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Quasar.HabboHotel.Groups;

namespace Quasar.Communication.Packets.Outgoing.Users
{
    class HabboGroupBadgesComposer : ServerPacket
    {
        public HabboGroupBadgesComposer(Dictionary<int, string> Badges)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
            base.WriteInteger(Badges.Count);
            foreach (KeyValuePair<int, string> Badge in Badges)
            {
                base.WriteInteger(Badge.Key);
                base.WriteString(Badge.Value);
            }
        }

        public HabboGroupBadgesComposer(Group Group)
            : base(ServerPacketHeader.HabboGroupBadgesMessageComposer)
        {
            base.WriteInteger(1);//count
            {
                base.WriteInteger(Group.Id);
                base.WriteString(Group.Badge);
            }
        }
    }
}
