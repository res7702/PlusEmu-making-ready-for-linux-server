using log4net;
using Quasar.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Catalog
{
    public class CatalogFrontPage
    {
        public string _oneN;
        public string _twoN;
        public string _threeN;
        public string _fourN;

        public string _oneI;
        public string _twoI;
        public string _threeI;
        public string _fourI;

        public string _onePL;
        public string _twoPL;
        public string _threePL;
        public string _fourPL;

        public CatalogFrontPage()
        {
            this.Init();
        }

        public void Init()
        {

            ILog log = LogManager.GetLogger("Quasar.Core.ConsoleCommandHandler");
            try
            {
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                Console.WriteLine(CurrentTime + "» Catalogus 4 voorpagina's");
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM catalog_frontpage WHERE id = 1");
                    dbClient.RunQuery();
                    DataRow row = dbClient.getRow();
                    var oneN = Convert.ToString(row["name"]);
                    var oneI = Convert.ToString(row["image"]);
                    var onePL = Convert.ToString(row["page_link"]);

                    dbClient.SetQuery("SELECT * FROM catalog_frontpage WHERE id = 2");
                    dbClient.RunQuery();
                    row = dbClient.getRow();
                    var twoN = Convert.ToString(row["name"]);
                    var twoI = Convert.ToString(row["image"]);
                    var twoPL = Convert.ToString(row["page_link"]);

                    dbClient.SetQuery("SELECT * FROM catalog_frontpage WHERE id = 3");
                    dbClient.RunQuery();
                    row = dbClient.getRow();
                    var threeN = Convert.ToString(row["name"]);
                    var threeI = Convert.ToString(row["image"]);
                    var threePL = Convert.ToString(row["page_link"]);

                    dbClient.SetQuery("SELECT * FROM catalog_frontpage WHERE id = 4");
                    dbClient.RunQuery();
                    row = dbClient.getRow();
                    var fourN = Convert.ToString(row["name"]);
                    var fourI = Convert.ToString(row["image"]);
                    var fourPL = Convert.ToString(row["page_link"]);

                    this._oneN = oneN;
                    this._oneI = oneI;
                    this._onePL = onePL;

                    this._twoN = twoN;
                    this._twoI = twoI;
                    this._twoPL = twoPL;

                    this._threeN = threeN;
                    this._threeI = threeI;
                    this._threePL = threePL;

                    this._fourN = fourN;
                    this._fourI = fourI;
                    this._fourPL = fourPL;

                }
            }
            catch (Exception e)
            {
                log.Debug(e.Message);
            }
        }
    }
}