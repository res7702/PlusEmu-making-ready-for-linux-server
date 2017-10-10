using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Quests;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;
using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Users
{
    class RespectUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom || Session.GetHabbo().GetStats().DailyRespectPoints <= 0)
                return;

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Packet.PopInt());
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo().Id == Session.GetHabbo().Id || User.IsBot)
                return;

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;
           
            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_RespectEarned", 1);

            Session.GetHabbo().GetStats().DailyRespectPoints -= 1;
            Session.GetHabbo().GetStats().RespectGiven += 1;
            User.GetClient().GetHabbo().GetStats().Respect += 1;

            // Respect Bubble
            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_respect", "Respect! Je hebt zojuist Respect gekregen."));

            User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("bubble_respect", "Je hebt al " + User.GetClient().GetHabbo().GetStats().Respect + " keer Respect gekregen."));

            if (Room.RespectNotificationsEnabled)
                Room.SendMessage(new RespectNotificationComposer(User.GetClient().GetHabbo().Id, User.GetClient().GetHabbo().GetStats().Respect));
            Room.SendMessage(new ActionComposer(ThisUser.VirtualId, 7));
        }
    }
}