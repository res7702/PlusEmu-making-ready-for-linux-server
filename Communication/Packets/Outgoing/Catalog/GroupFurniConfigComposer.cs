using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Groups;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    class GroupFurniConfigComposer : ServerPacket
    {
        public GroupFurniConfigComposer(ICollection<Group> Groups)
            : base(ServerPacketHeader.GroupFurniConfigMessageComposer)
        {
            base.WriteInteger(Groups.Count);
            foreach (Group Group in Groups)
            {
                base.WriteInteger(Group.Id);
                base.WriteString(Group.Name);
                base.WriteString(Group.Badge);
                base.WriteString((QuasarEnvironment.GetGame().GetGroupManager().SymbolColours.ContainsKey(Group.Colour1)) ? QuasarEnvironment.GetGame().GetGroupManager().SymbolColours[Group.Colour1].Colour : "4f8a00"); // Group Colour 1
                base.WriteString((QuasarEnvironment.GetGame().GetGroupManager().BackGroundColours.ContainsKey(Group.Colour2)) ? QuasarEnvironment.GetGame().GetGroupManager().BackGroundColours[Group.Colour2].Colour : "4f8a00"); // Group Colour 2            
                base.WriteBoolean(false);
                base.WriteInteger(Group.CreatorId);
                base.WriteBoolean(Group.ForumEnabled);
            }
        }
    }
}
