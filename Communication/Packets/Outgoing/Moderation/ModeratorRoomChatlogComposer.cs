using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;

using Quasar.Utilities;
using Quasar.HabboHotel.Cache;

namespace Quasar.Communication.Packets.Outgoing.Moderation
{
    class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room Room)
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {
            base.WriteByte(1);
            base.WriteShort(2);//Count
           base.WriteString("roomName");
            base.WriteByte(2);
           base.WriteString(Room.Name);
           base.WriteString("roomId");
            base.WriteByte(1);
            base.WriteInteger(Room.Id);

            DataTable Table = null;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `chatlogs` WHERE `room_id` = @rid ORDER BY `id` DESC LIMIT 250");
                dbClient.AddParameter("rid", Room.Id);
                Table = dbClient.getTable();
            }

            base.WriteShort(Table.Rows.Count);
            if (Table != null)
            {
                foreach (DataRow Row in Table.Rows)
                {
                    UserCache Habbo = QuasarEnvironment.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(Row["user_id"]));

                    if (Habbo == null)
                    {
                        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dtDateTime = dtDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        base.WriteString(dtDateTime.Hour + ":" + dtDateTime.Minute);
                        base.WriteInteger(-1);
                        base.WriteString("Onbekende gebruiker");
                        base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"])) ? "*bericht met niks erin*" : Convert.ToString(Row["message"]));
                        base.WriteBoolean(false);
                    }
                    else
                    {
                        DateTime dDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        dDateTime = dDateTime.AddSeconds(Convert.ToInt32(Row["timestamp"])).ToLocalTime();

                        base.WriteString(dDateTime.Hour + ":" + dDateTime.Minute);
                        base.WriteInteger(Habbo.Id);
                        base.WriteString(Habbo.Username);
                        base.WriteString(string.IsNullOrWhiteSpace(Convert.ToString(Row["message"]) )? "*spatie*" : Convert.ToString(Row["message"]));
                        base.WriteBoolean(false);
                    }
                }
            }
        }
    }
}
