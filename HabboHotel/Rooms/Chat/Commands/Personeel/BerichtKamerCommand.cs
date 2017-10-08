using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BerichtKamerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_berichtkamer"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur een bericht naar iedereen in de kamer."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een bericht in te vullen.", 34);
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_alert") && Room.OwnerId != Session.GetHabbo().Id)
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                    continue;


                RoomUser.GetClient().SendMessage(new RoomNotificationComposer("Bericht",
                     "De kamer eigenaar heeft een bericht verzonden naar iedereen in de kamer.\n\n<b>Mededeling:</b>\n\n" + Message + "\n\n- " + Session.GetHabbo().Username,
                     "kamerbericht"));
            }
            Session.SendWhisper("Je hebt een bericht verstuurd aan iedereen in de kamer.", 34);
        }
    }
}
