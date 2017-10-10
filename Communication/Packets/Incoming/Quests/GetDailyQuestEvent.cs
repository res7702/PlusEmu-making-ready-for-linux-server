using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.LandingView;

namespace Quasar.Communication.Packets.Incoming.Quests
{
    class GetDailyQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int UsersOnline = QuasarEnvironment.GetGame().GetClientManager().Count;

            Session.SendMessage(new ConcurrentUsersGoalProgressComposer(UsersOnline));
        }
    }
}
