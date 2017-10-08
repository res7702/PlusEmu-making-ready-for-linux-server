/*using System.Linq;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Incoming;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Quests;

using Quasar.Database.Interfaces;
using log4net;
using Quasar.HabboHotel.Items;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;

#region OLD CAMERA
namespace Quasar.HabboHotel.Camera
{
    public class CameraPhotoManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Camera.CameraPhotoManager");

        private Dictionary<int, CameraPhotoPreview> _previews;

        private int _maxPreviewCacheCount = 1000;
        private int _purchaseCoinsPrice = 3;
        private int _purchaseDucketsPrice = 3;
        private int _publishDucketsPrice = 3;

        private ItemData _photoPoster;

        public int PurchaseCoinsPrice
        {
            get
            {
                return this._purchaseCoinsPrice;
            }
        }

        public int PurchaseDucketsPrice
        {
            get
            {
                return this._purchaseDucketsPrice;
            }
        }

        public int PublishDucketsPrice
        {
            get
            {
                return this._publishDucketsPrice;
            }
        }

        public void Init(ItemDataManager itemDataManager)
        {
            if (QuasarEnvironment.GetDBConfig().DBData.ContainsKey("camera.photo.purchase.price.coins"))
            {
                this._purchaseCoinsPrice = int.Parse(QuasarEnvironment.GetDBConfig().DBData["camera.photo.purchase.price.coins"]);
            }

            if (QuasarEnvironment.GetDBConfig().DBData.ContainsKey("camera.photo.purchase.price.duckets"))
            {
                this._purchaseDucketsPrice = int.Parse(QuasarEnvironment.GetDBConfig().DBData["camera.photo.purchase.price.duckets"]);
            }

            if (QuasarEnvironment.GetDBConfig().DBData.ContainsKey("camera.photo.publish.price.duckets"))
            {
                this._publishDucketsPrice = int.Parse(QuasarEnvironment.GetDBConfig().DBData["camera.photo.publish.price.duckets"]);
            }
        }
    }
}
#endregion*/

using Quasar.HabboHotel.Camera;

