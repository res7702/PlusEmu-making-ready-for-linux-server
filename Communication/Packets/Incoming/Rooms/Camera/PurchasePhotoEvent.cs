//using System;

//using Quasar.HabboHotel.Rooms;
//using Quasar.HabboHotel.GameClients;
//using Quasar.Communication.Packets.Outgoing.Rooms.Camera;
//using Quasar.HabboHotel.Camera;
//using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
//using Quasar.HabboHotel.Items;
//using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
//using Quasar.Database.Interfaces;
//using Quasar.Utilities;

//namespace Quasar.Communication.Packets.Incoming.Rooms.Camera
//{
//    public class PurchasePhotoEvent : IPacketEvent
//    {
//        public void Parse(GameClient Session, ClientPacket Packet)
//        {
//            if (!Session.GetHabbo().InRoom || Session.GetHabbo().Credits < QuasarEnvironment.GetGame().GetCameraManager().PurchaseCoinsPrice || Session.GetHabbo().Duckets < QuasarEnvironment.GetGame().GetCameraManager().PurchaseDucketsPrice)
//                return;

//            Room Room = Session.GetHabbo().CurrentRoom;

//            if (Room == null)
//                return;

//            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

//            if (User == null || User.LastPhotoPreview == null)
//                return;

//            CameraPhotoPreview preview = User.LastPhotoPreview;

//            if (QuasarEnvironment.GetGame().GetCameraManager().PurchaseCoinsPrice > 0)
//            {
//                Session.GetHabbo().Credits -= QuasarEnvironment.GetGame().GetCameraManager().PurchaseCoinsPrice;
//                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
//            }

//            if (QuasarEnvironment.GetGame().GetCameraManager().PurchaseDucketsPrice > 0)
//            {
//                Session.GetHabbo().Duckets -= QuasarEnvironment.GetGame().GetCameraManager().PurchaseDucketsPrice;
//                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
//            }

//            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
//            {
//                dbClient.RunQuery("UPDATE `camera_photos` SET `file_state` = 'purchased' WHERE `id` = '" + preview.Id + "' LIMIT 1");
//            }

//            Item photoPoster = ItemFactory.CreateSingleItemNullable(QuasarEnvironment.GetGame().GetCameraManager().PhotoPoster, Session.GetHabbo(),
//"{\"w\":\"" + StringCharFilter.EscapeJSONString(QuasarEnvironment.GetGame().GetCameraManager().GetPath(CameraPhotoType.PURCHASED, preview.Id, preview.CreatorId)) + "\", \"n\":\"" + StringCharFilter.EscapeJSONString(Session.GetHabbo().Username) + "\", \"s\":\"" + Session.GetHabbo().Id + "\", \"u\":\"" + preview.Id + "\", \"t\":\"" + preview.CreatedAt + "\"}",
//            "");

//            if (photoPoster != null)
//            {
//                Session.GetHabbo().GetInventoryComponent().TryAddItem(photoPoster);

//                Session.SendMessage(new FurniListAddComposer(photoPoster));
//                Session.SendMessage(new FurniListUpdateComposer());
//            }
//            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CameraPhotoCount", 1);
//            Session.SendMessage(new CameraPhotoPurchaseOkComposer());
//        }
//    }
//}