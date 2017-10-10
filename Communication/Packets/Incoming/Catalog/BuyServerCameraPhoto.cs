using Quasar.Communication.Packets.Incoming.Rooms.Camera;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Rooms.Camera;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Camera;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Items;
using Quasar.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class BuyServerCameraPhoto : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket paket)
        {
            if (!Session.GetHabbo().lastPhotoPreview.Contains("-"))
                return;

            string roomId = Session.GetHabbo().lastPhotoPreview.Split('-')[0];
            string timestamp = Session.GetHabbo().lastPhotoPreview.Split('-')[1];
            string md5image = URLPost.GetMD5(Session.GetHabbo().lastPhotoPreview);
            ItemData Item = null;
            if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(8763, out Item)) // Ok I don't know why your item is this crazy id? Change it here, anyways. 
                return;
            if (Item == null)
                return;


            Item photoPoster = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "{\"timestamp\":\"" + timestamp + "\", \"id\":\"" + md5image + "\"}", "");

            if (photoPoster != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(photoPoster);

                Session.SendMessage(new FurniListAddComposer(photoPoster));
                Session.SendMessage(new FurniListUpdateComposer());
            }

            Session.SendMessage(new BuyPhoto());

            Session.GetHabbo().GetInventoryComponent().UpdateItems(false);

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO items_camera VALUES (@id, '" + Session.GetHabbo().Id + "',@creator_name, '" + roomId + "','" + timestamp + "')");
                dbClient.AddParameter("id", md5image);
                dbClient.AddParameter("creator_name", Session.GetHabbo().Username);
                dbClient.RunQuery();
            }
        }
    }
}
