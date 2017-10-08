using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using log4net;
using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Global
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;
        string HotelName = QuasarEnvironment.GetConfig().data["hotel.name"];

        private Timer _timer;
        
        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = "Emulator | 0 Habbis | 0 Kamers | 0 Dagen";

            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Console.WriteLine(CurrentTime + "» Hotel Serverinformatie");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
        }

        private void UpdateOnlineUsers()
        {
            TimeSpan Uptime = DateTime.Now - QuasarEnvironment.ServerStarted;

            int UsersOnline = Convert.ToInt32(QuasarEnvironment.GetGame().GetClientManager().Count);
            int RoomCount = QuasarEnvironment.GetGame().GetRoomManager().Count;

            Console.Title = "Emulator | " + UsersOnline + " Habbis | " + RoomCount + " Kamers | " + Uptime.Days + " Dagen";

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `loaded_rooms` = @loadedRooms LIMIT 1;");
                dbClient.AddParameter("users", UsersOnline);
                dbClient.AddParameter("loadedRooms", RoomCount);
                dbClient.RunQuery();
            }

            int userPeak;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `userpeak` FROM `server_status` LIMIT 1");
                userPeak = dbClient.getInteger();
            }
            if (UsersOnline > userPeak)
            {
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `server_status` SET `userpeak` = @userpeak LIMIT 1;");
                    dbClient.AddParameter("userpeak", UsersOnline);
                    dbClient.RunQuery();
                }
            }
        }


        public void Dispose()
        {
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
