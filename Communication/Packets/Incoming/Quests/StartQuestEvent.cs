using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Incoming.Quests
{
    class StartQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int QuestId = Packet.PopInt();

            QuasarEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, QuestId);
        }
    }
}
