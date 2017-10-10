using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Groups
{
    class ThreadUpdatedComposer : ServerPacket
    {
        public ThreadUpdatedComposer(GameClient Session, GroupForumThread Thread)
            : base(ServerPacketHeader.ThreadUpdatedMessageComposer)
        {
            base.WriteInteger(Thread.ParentForum.Id);

            Thread.SerializeData(Session, this);
        }
    }
}
