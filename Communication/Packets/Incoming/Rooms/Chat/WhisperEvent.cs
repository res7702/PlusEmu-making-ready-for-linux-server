using System;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Core;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Incoming;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Utilities;
using Quasar.HabboHotel.Rooms.Chat.Styles;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Rooms.Chat
{
    public class WhisperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool") && Room.CheckMute(Session))
            {
                Session.SendWhisper("Oeps! Je hebt een spreekverbod.");
                return;
            }

            if (QuasarEnvironment.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
                return;

            string Params = Packet.PopString();
            string ToUser = Params.Split(' ')[0];
            string Message = Params.Substring(ToUser.Length + 1);
            int Colour = Packet.PopInt();

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(ToUser);
            if (User2 == null)
                return;

            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
                return;
            }

            ChatStyle Style = null;

            if (!QuasarEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
                Colour = 0;

            User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                int MuteTime;
                if (User.IncrementAndCheckFlood(out MuteTime))
                {
                    Session.SendMessage(new FloodControlComposer(MuteTime));
                    return;
                }
            }

            if (!User2.GetClient().GetHabbo().ReceiveWhispers && !Session.GetHabbo().GetPermissions().HasRight("room_whisper_override"))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                return;
            }

            QuasarEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new Quasar.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(Session.GetHabbo().Id, Room.Id, ": " + Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));

            Room.AddChatlog(Session.GetHabbo().Id, ": " + Message);

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Room _room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(2);
                Session.GetHabbo().PrepareRoom(_room.Id, "2");
                Session.SendMessage(new RoomCustomizedAlertComposer("Het woord '" + word + "' is verboden."));
                QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("bubble_filter", "Filter bericht\n\nHabbis: " + Session.GetHabbo().Username + "\nWoord: " + word + "\nType: Chatconsole", ""));
                return;
            }

            QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

            User.UnIdle();
            User.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));

            if (User2 != null && !User2.IsBot && User2.UserId != User.UserId)
            {
                if (!User2.GetClient().GetHabbo().MutedUsers.Contains(Session.GetHabbo().Id))
                {
                    User2.GetClient().SendMessage(new WhisperComposer(User.VirtualId, Message, 0, User.LastBubble));
                }
            }

            List<RoomUser> ToNotify = Room.GetRoomUserManager().GetRoomUserByRank(5);
            if (ToNotify.Count > 0)
            {
                foreach (RoomUser user in ToNotify)
                {
                    if (user != null && user.HabboId != User2.HabboId && user.HabboId != User.HabboId)
                    {
                        if (user.GetClient() != null && user.GetClient().GetHabbo() != null && !user.GetClient().GetHabbo().IgnorePublicWhispers)
                        {
                            user.GetClient().SendMessage(new WhisperComposer(User.VirtualId, "[Fluistert naar " + ToUser + "] " + Message, 0, User.LastBubble));
                        }
                    }
                }
            }
        }
    }
}