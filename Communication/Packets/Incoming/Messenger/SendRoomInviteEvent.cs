using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Utilities;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Messenger;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.Communication.Packets.Incoming.Messenger
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Oeps! Je hebt een spreekverbod."));
                return;
            }

            int Amount = Packet.PopInt();
            if (Amount > 500)
                return; // don't send at all

            List<int> Targets = new List<int>();
            for (int i = 0; i < Amount; i++)
            {
                int uid = Packet.PopInt();
                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    Targets.Add(uid);
                }
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 121)
                Message = Message.Substring(0, 121);

            /*
            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 25;
                    Session.SendNotification("¡Has sido silenciad@ mientras un moderador revisa tu caso, al parecer nombraste un hotel! Aviso " + Session.GetHabbo().BannedPhraseCount + "/3");
                    QuasarEnvironment.GetGame().GetClientManager().StaffAlert(new RoomNotificationComposer("Alerta de publicista:",
                        "Atención, se ha mencionado la palabra <b>" + word.ToUpper() + "</b><br><br><b>Frase:</b><br><i>" + Message +
                        "</i>.<br><br><b>Tipo</b><br>Spam por invitación en consola.\r\n" + "- Este usuario: <b>" +
                        Session.GetHabbo().Username + "</b>", "zpam", "", ""));
                }
                if (Session.GetHabbo().BannedPhraseCount >= 3)
                {
                    QuasarEnvironment.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Baneado por hacer Spam con la Frase (" + Message + ")", (QuasarEnvironment.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }
            */

            foreach (int UserId in Targets)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                    continue;

                GameClient Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().AllowMessengerInvites == true || Client.GetHabbo().AllowConsoleMessages == false)
                    continue;

                Client.SendMessage(new RoomInviteComposer(Session.GetHabbo().Id, Message));
                Client.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", "" + Session.GetHabbo().Username + " heeft een invite gestuurd: " + Message + ".", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));

            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES ('" + Session.GetHabbo().Id + "', @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }
    }
}