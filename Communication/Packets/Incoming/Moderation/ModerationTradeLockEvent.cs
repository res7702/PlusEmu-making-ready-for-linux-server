using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Users;


namespace Quasar.Communication.Packets.Incoming.Moderation
{
    class ModerationTradeLockEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_trade_lock"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Days = (Packet.PopInt() / 1440);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();

            double Length = (QuasarEnvironment.GetUnixTimestamp() + (Days * 86400));

            Habbo Habbo = QuasarEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker kan niet worden gevonden.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_trade_lock") && !Session.GetHabbo().GetPermissions().HasRight("mod_trade_lock_any"))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze gebruiker een ruil blokkering op te leggen.");
                return;
            }

            if (Days < 1)
                Days = 1;

            if (Days > 365)
                Days = 365;

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '" + Length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TradingLockExpiry = Length;
                Habbo.GetClient().SendNotification("Je hebt een ruilban voor " + Days + " dag(en)!\r\rReden:\r\r" + Message);
            }
        }
    }
}
