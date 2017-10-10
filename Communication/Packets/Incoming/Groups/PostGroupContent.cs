using Quasar.Communication.Packets.Outgoing.Groups;
using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    class PostGroupContentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt();
            var ThreadId = Packet.PopInt();
            var Caption = Packet.PopString();
            var Message = Packet.PopString();

            var Forum = QuasarEnvironment.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
                Session.SendNotification("Oeps! Dit forum kan niet worden gevonden.");
                return;
            }
            var e = "";
            var IsNewThread = ThreadId == 0;
            if (IsNewThread)
            {

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions)) != "")
                {
                    Session.SendNotification("Oeps! Er is wat mis gegaan: " + e);
                    return;
                }

                var Thread = Forum.CreateThread(Session.GetHabbo().Id, Caption);
                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);

                Session.SendMessage(new ThreadCreatedComposer(Session, Thread));

                Session.SendMessage(new ThreadDataComposer(Thread, 0, 2000));

                //Session.SendMessage(new PostUpdatedComposer(Session, Post));
                //Session.SendMessage(new ThreadReplyComposer(Session, Post));

                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "", 1); //new thread post

                Thread.AddView(Session.GetHabbo().Id, 1);

            }
            else
            {
                var Thread = Forum.GetThread(ThreadId);
                if (Thread == null)
                {
                    Session.SendNotification("Oeps! Het forum kon niet worden gevonden.");
                    return;
                }

                if (Thread.Locked && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) != "")
                {
                    Session.SendNotification("Oeps! Je kan niet reageren op dit forum.");
                    return;
                }

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)) != "")
                {
                    Session.SendNotification("Oeps! Er is wat mis gegaan: " + e);
                    return;
                }

                var Post = Thread.CreatePost(Session.GetHabbo().Id, Message);
                Session.SendMessage(new ThreadReplyComposer(Session, Post));
            }


        }
    }
}
