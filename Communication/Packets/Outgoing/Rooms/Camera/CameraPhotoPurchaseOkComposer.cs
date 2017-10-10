using System;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraPhotoPurchaseOkComposer : ServerPacket
    {
        public CameraPhotoPurchaseOkComposer()
            : base(ServerPacketHeader.CameraPhotoPurchaseOkComposer)
        {
        }
    }
}