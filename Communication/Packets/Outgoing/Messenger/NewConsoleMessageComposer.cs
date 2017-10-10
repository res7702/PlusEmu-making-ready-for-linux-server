using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Messenger
{
    class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            base.WriteInteger(Sender);
            base.WriteString(Message);
            base.WriteInteger(Time);
        }

        public NewConsoleMessageComposer(int GroupId, string Message, int Time, int UserId, string Username, string figure)
           : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            base.WriteInteger(-GroupId);
            base.WriteString(Message);
            base.WriteInteger(Time);
            base.WriteString(Username + "/" + figure + "/" + UserId);
        }

        class FuckingConsoleMessageComposer : ServerPacket
        {
            public FuckingConsoleMessageComposer(int Sender, string Message, string Data)
                : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
            {
                base.WriteInteger(Sender);
                base.WriteString(Message);
                base.WriteInteger(0);
                base.WriteString(Data);
            }
        }
    }
}