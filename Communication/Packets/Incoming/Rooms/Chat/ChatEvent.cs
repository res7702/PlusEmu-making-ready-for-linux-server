using System;

using Quasar.Core;
using Quasar.Communication.Packets.Incoming;
using Quasar.Utilities;
using Quasar.HabboHotel.Global;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms.Chat.Logs;
using Quasar.Communication.Packets.Outgoing.Messenger;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.HabboHotel.Items.Data;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.Rooms.Chat.Styles;



namespace Quasar.Communication.Packets.Incoming.Rooms.Chat
{
    public class ChatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 100)
                Message = Message.Substring(0, 100);

            int Colour = Packet.PopInt();

            ChatStyle Style = null;
            if (!QuasarEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
                Colour = 0;

            User.UnIdle();

            if (QuasarEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                return;

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && Room.CheckMute(Session))
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Deze kamer heeft een spraakverbod."));
                return;
            }

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (Room.GetWired().TriggerEvent(HabboHotel.Items.Wired.WiredBoxType.TriggerUserSays, Session.GetHabbo(), Message))
            {
                return;

            }
            else if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                int MuteTime;
                if (User.IncrementAndCheckFlood(out MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }


            if (Message.StartsWith(":", StringComparison.CurrentCulture))
            {
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ExploreCommand", 1);
            }

        

            if (Message.StartsWith(":", StringComparison.CurrentCulture) && QuasarEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
             return;

            QuasarEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));
            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Het woord '" + word + "' is verboden."));
                QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("bubble_filter", "Filter bericht\n\nHabbis: " + Session.GetHabbo().Username + "\nWoord: " + word + "\nType: Chat (kamer)", ""));
                return;
            }
            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

            User.OnChat(User.LastBubble, Message, false);

            if (Message.Length > 20)
            {
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_ChatSpeeches", 1);
            }
        }
    }
}