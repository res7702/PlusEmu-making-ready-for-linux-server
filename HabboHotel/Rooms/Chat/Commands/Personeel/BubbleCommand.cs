using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms.Chat.Styles;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BubbleCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_bubble"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Gebruik een custom spraakwolkje om mee te typen."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps, je bent vergeten een id in te voeren!", 34);
                return;
            }

            int Bubble = 0;
            if (!int.TryParse(Params[1].ToString(), out Bubble))
            {
                Session.SendWhisper("Vul een geldig getal in.", 34);
                return;
            }
            if (Session.GetHabbo().Rank < 5)
            {
                if (Convert.ToInt32(Params[1]) == 37 || Convert.ToInt32(Params[1]) == 31 || Convert.ToInt32(Params[1]) == 30 || Convert.ToInt32(Params[1]) == 23 || Convert.ToInt32(Params[1]) == 34 || Convert.ToInt32(Params[1]) == 33)
                {
                    Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                    return;
                }
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("Je praat nu met het het spraakwolkje " + Bubble);
        }
    }
}