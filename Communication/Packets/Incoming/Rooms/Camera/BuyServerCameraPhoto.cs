//using Quasar.Communication.Packets.Outgoing;
//using Quasar.HabboHotel.GameClients;

//namespace Quasar.Communication.Packets.Incoming.Rooms.Camera
//{
//    public class BuyServerCameraPhoto : ServerPacket
//    {
//        public BuyServerCameraPhoto()
//            : base(ServerPacketHeader.BuyPhoto)
//        {

//        }
//    }

//    public class BuyServerCameraPhotoEvent : IPacketEvent
//    {
//        public void Parse(GameClient Session, ClientPacket paket)
//        {
//            if (!Session.GetHabbo().lastPhotoPreview.Contains("-"))
//                return;

//            string roomId = Session.GetHabbo().lastPhotoPreview.Split('-')[0];
//            string timestamp = Session.GetHabbo().lastPhotoPreview.Split('-')[1];
//            string md5image = URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview);

//            Item Item = MarshMallowEnvironment.GetGame().GetItemManager().GetItem(EmuSettings.CAMERA_BASEID); //
//            if (Item == null)
//                return;

//            Session.SendMessage(new BuyServerCameraPhoto());

//            MarshMallowEnvironment.GetGame().GetCatalog().DeliverItems(Session, Item, 1, "{\"timestamp\":\"" + timestamp + "\", \"id\":\"" + md5image + "\"}", true, 0);
//            Session.GetHabbo().GetInventoryComponent().UpdateItems(false);

//            using (IQueryAdapter dbClient = MarshMallowEnvironment.GetDatabaseManager().getQueryreactor())
//            {
//                dbClient.setQuery("INSERT INTO items_camera VALUES (@id, '" + Session.GetHabbo().Id + "',@creator_name, '" + roomId + "','" + timestamp + "')");
//                dbClient.addParameter("id", md5image);
//                dbClient.addParameter("creator_name", Session.GetHabbo().Username);
//                dbClient.runQuery();
//            }

            
//        }
//    }
//}