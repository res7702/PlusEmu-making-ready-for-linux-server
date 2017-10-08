﻿using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.LandingView.Promotions;
using log4net;

namespace Quasar.HabboHotel.LandingView
{
    public class LandingViewManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.LandingView.LandingViewManager");

        private Dictionary<int, Promotion> _promotionItems;

        public LandingViewManager()
        {
            this._promotionItems = new Dictionary<int, Promotion>();

            this.LoadPromotions();
        }

        public void LoadPromotions()
        {
            if (this._promotionItems.Count > 0)
                this._promotionItems.Clear();

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `server_landing` ORDER BY `id` DESC");
                DataTable GetData = dbClient.getTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        this._promotionItems.Add(Convert.ToInt32(Row[0]), new Promotion((int)Row[0], Row[1].ToString(), Row[2].ToString(), Row[3].ToString(), Convert.ToInt32(Row[4]), Row[5].ToString(), Row[6].ToString()));
                    }
                }
            }


            //string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            //Console.WriteLine(CurrentTime + "» Hotel Overzicht");
        }

        public ICollection<Promotion> GetPromotionItems()
        {
            return this._promotionItems.Values;
        }
    }
}