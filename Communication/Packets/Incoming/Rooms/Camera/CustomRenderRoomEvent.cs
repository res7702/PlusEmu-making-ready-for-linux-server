//using System;

//using Quasar.HabboHotel.Rooms;
//using Quasar.HabboHotel.GameClients;
//using Quasar.Communication.Packets.Outgoing.Rooms.Camera;
//using Quasar.HabboHotel.Camera;

//namespace Quasar.Communication.Packets.Incoming.Rooms.Camera
//{
//    public class CustomRenderRoomEvent : IPacketEvent
//    {
//        public void Parse(GameClient Session, ClientPacket Packet)
//        {
//            if (!Session.GetHabbo().InRoom)
//                return;

//            Room Room = Session.GetHabbo().CurrentRoom;

//            if (Room == null)
//                return;

//            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

//            if (User == null)
//                return;

//            int photoId;

//            if (!int.TryParse(Packet.PopString(), out photoId) || photoId < 0)
//            {
//                return;
//            }

//            CameraPhotoPreview preview = QuasarEnvironment.GetGame().GetCameraManager().GetPreview(photoId);

//            if (preview == null || preview.CreatorId != Session.GetHabbo().Id)
//            {
//                return;
//            }

//            User.LastPhotoPreview = preview;
//            Session.SendMessage(new CameraPhotoPreviewComposer(QuasarEnvironment.GetGame().GetCameraManager().GetPath(CameraPhotoType.PREVIEW, preview.Id, preview.CreatorId)));
//        }
//    }
//}