using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Groups;
using Quasar.Communication.Packets.Outgoing.Groups;
using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Groups
{
    class UpdateGroupBadgeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();

            Group Group = null;
            if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

             if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            int Count = Packet.PopInt();
            int Current = 1;
        
            string x;
            string newBadge = "";
            while (Current <= Count)
            {
                int Id = Packet.PopInt();
                int Colour = Packet.PopInt();
                int Pos = Packet.PopInt();
                if (Current == 1)
                    x = "b" + ((Id < 10) ? "0" + Id.ToString() : Id.ToString()) +     ((Colour < 10) ? "0" + Colour.ToString() : Colour.ToString()) + Pos;
                else
                    x = "s" + ((Id < 10) ? "0" + Id.ToString() : Id.ToString()) +   ((Colour < 10) ? "0" + Colour.ToString() : Colour.ToString()) + Pos;
                newBadge += QuasarEnvironment.GetGame().GetGroupManager().CheckActiveSymbol(x);
                Current++;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE groups SET badge = @badge WHERE id=" + Group.Id + " LIMIT 1");
                dbClient.AddParameter("badge", newBadge);
                dbClient.RunQuery();
            }

            Group.Badge = (string.IsNullOrWhiteSpace(newBadge) ? "b05114s06114" : newBadge);
            Session.SendMessage(new GroupInfoComposer(Group, Session));
        }
    }
}
