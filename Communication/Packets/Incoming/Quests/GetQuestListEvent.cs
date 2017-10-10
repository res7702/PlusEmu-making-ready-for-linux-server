using System.Collections.Generic;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Quests;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            QuasarEnvironment.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}