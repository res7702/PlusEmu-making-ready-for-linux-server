using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Incoming.Moderation
{
    class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_mute"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Length = (Packet.PopInt() * 60);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();

            Habbo Habbo = QuasarEnvironment.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker kan niet worden gevonden.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_mute") && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_any"))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze gebruiker een spreekverbod op te leggen.");
                return;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `time_muted` = '" + Length + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TimeMuted = Length;
                //Habbo.GetClient().SendNotification("Usted ha sido silenciado por tener un mal comportamiento en Habbi Hotel. Cómportese para evitar futuras sanciones");
                Habbo.GetClient().SendNotification("Je hebt een spreekverbod  " + Length + " seconden!");
            }
        }
    }
}

