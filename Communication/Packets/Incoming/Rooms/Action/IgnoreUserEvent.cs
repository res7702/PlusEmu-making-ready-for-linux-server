using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Action;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class IgnoreUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            String Username = Packet.PopString();
            Habbo User = QuasarEnvironment.GetHabboByUsername(Username);
            if (User == null || Session.GetHabbo().MutedUsers.Contains(User.Id) || User.GetPermissions().HasRight("mod_tool"))
                return;

            Session.GetHabbo().MutedUsers.Add(User.Id);
            Session.SendMessage(new IgnoreStatusComposer(1, Username));

            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModIgnoreSeen", 1);
        }
    }
}
