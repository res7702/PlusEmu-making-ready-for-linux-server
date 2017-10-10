using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quasar.Utilities;
using Quasar.Core;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Rooms.Avatar
{
    class ChangeMottoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendNotification("Oeps! Je hebt een spreekverbod. Het veranderen van je status kan niet..");
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastMottoUpdateTime).TotalSeconds <= 2.0)
            {
                Session.GetHabbo().MottoUpdateWarnings += 1;
                if (Session.GetHabbo().MottoUpdateWarnings >= 25)
                    Session.GetHabbo().SessionMottoBlocked = true;
                return;
            }

            if (Session.GetHabbo().SessionMottoBlocked)
                return;

            Session.GetHabbo().LastMottoUpdateTime = DateTime.Now;

            string newMotto = StringCharFilter.Escape(Packet.PopString().Trim());

            if (newMotto.Length > 100)
                newMotto = newMotto.Substring(0, 100);

            if (newMotto == Session.GetHabbo().Motto)
                return;

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override"))
                newMotto = QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(newMotto, out word) ? "Spam" : newMotto;

            Session.GetHabbo().Motto = newMotto;

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `motto` = @motto WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("motto", newMotto);
                dbClient.RunQuery();
            }

            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.PROFILE_CHANGE_MOTTO);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Motto", 1);

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;
                if (Room == null)
                    return;

                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User == null || User.GetClient() == null)
                    return;

                Room.SendMessage(new UserChangeComposer(User, false));
            }
        }
    }
}
