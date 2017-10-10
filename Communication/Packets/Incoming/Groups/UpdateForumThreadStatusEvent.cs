using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    public class UpdateForumThreadStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumID = Packet.PopInt();
            var ThreadID = Packet.PopInt();
            var Pinned = Packet.PopBoolean();
            var Locked = Packet.PopBoolean();


            var forum = QuasarEnvironment.GetGame().GetGroupForumManager().GetForum(ForumID);
            var thread = forum.GetThread(ThreadID);

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
                Session.SendNotification(("Oeps! Je hebt niet de bevoegdheid om dit te doen."));
                return;
            }

            bool isPining = thread.Pinned != Pinned,
                isLocking = thread.Locked != Locked;

            thread.Pinned = Pinned;
            thread.Locked = Locked;

            thread.Save();

            Session.SendMessage(new Outgoing.Groups.ThreadUpdatedComposer(Session, thread));

            if (isPining)
                if (Pinned)
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("Forum thread staat vast!"));
                else
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("Forum thread is weer los!"));

            if (isLocking)
                if (Locked)
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("Forum thread is gesloten!"));
                else
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("Forum thread is open!"));

        }
    }
}
