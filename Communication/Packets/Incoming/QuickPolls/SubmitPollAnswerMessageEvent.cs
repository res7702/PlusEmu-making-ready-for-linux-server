using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using Quasar.HabboHotel.Rooms;
using System.Text;
using System.Threading.Tasks;
using Quasar.Communication.Packets.Outgoing.Rooms.Poll;

namespace Quasar.Communication.Packets.Incoming.QuickPolls
{
    class SubmitPollAnswerMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            int pollId = Packet.PopInt();
            int questionId = Packet.PopInt();
            int count = Packet.PopInt();

            String answer = Packet.PopString();
            //DEBUG MADAFAKA
            if (questionId == -1)
            {
                if (Session == null || Session.GetHabbo() == null)
                    return;

                Room room = Session.GetHabbo().CurrentRoom;
                if (room == null)
                    return;

                if (room.poolQuestion == string.Empty)
                {
                    return;
                }

                if (room.yesPoolAnswers.Contains(Session.GetHabbo().Id) || room.noPoolAnswers.Contains(Session.GetHabbo().Id))
                {
                    return;
                }

                if (answer.Equals("1"))
                {
                    room.yesPoolAnswers.Add(Session.GetHabbo().Id);
                    Console.WriteLine("Gebruiker " + Session.GetHabbo().Username + " heeft positief op de poll gestemd.");
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PollVote", 1);
                }
                else
                {
                    room.noPoolAnswers.Add(Session.GetHabbo().Id);

                    Console.WriteLine("Gebruiker " + Session.GetHabbo().Username + " heeft negatief op de poll gestemd.");
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PollVote", 1);
                }

                room.SendMessage(new QuickPollResultMessageComposer(Session.GetHabbo().Id, answer, room.yesPoolAnswers.Count, room.noPoolAnswers.Count));
                return;
            }
        }
    }
}

       
