using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Groups;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new BadgeEditorPartsComposer(
                QuasarEnvironment.GetGame().GetGroupManager().Bases,
                QuasarEnvironment.GetGame().GetGroupManager().Symbols,
                QuasarEnvironment.GetGame().GetGroupManager().BaseColours,
                QuasarEnvironment.GetGame().GetGroupManager().SymbolColours,
                QuasarEnvironment.GetGame().GetGroupManager().BackGroundColours));
       
        }
    }
}
