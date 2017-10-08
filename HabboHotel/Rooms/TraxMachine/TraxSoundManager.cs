using log4net;
using Quasar.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Quasar.HabboHotel.Rooms.TraxMachine
{
    public class TraxSoundManager
    {
        public static List<TraxMusicData> Songs = new List<TraxMusicData>();

        private static ILog Log = LogManager.GetLogger(" Quasar.HabboHotel.Rooms.TraxMachine");

        public static void Init()
        {
            TraxSoundManager.Songs.Clear();
            DataTable table;
            using (IQueryAdapter queryReactor = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunQuery("SELECT * FROM jukebox_songs_data");
                table = queryReactor.getTable();
            }
            foreach (DataRow row in table.Rows)
            {
                TraxSoundManager.Songs.Add(TraxMusicData.Parse(row));
            }
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Console.WriteLine(CurrentTime + "» Jukebox " + TraxSoundManager.Songs.Count + " nummers");
         }

        public static TraxMusicData GetMusic(int id)
        {
            TraxMusicData result;
            foreach (TraxMusicData current in TraxSoundManager.Songs)
            {
                bool flag = current.Id == id;
                if (flag)
                {
                    result = current;
                    return result;
                }
            }
            result = null;
            return result;
        }
    }
}
