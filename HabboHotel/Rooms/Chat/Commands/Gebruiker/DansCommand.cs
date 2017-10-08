using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class DansCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_dans"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Laat je Habbis dansen (1-4)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een dans-id (1-4) in te voeren.");
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("Je hebt maar vier verschillende soorten dansen!");
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, DanceId));
            }
            else
                Session.SendWhisper("Oeps! Je moet wel een geldig dans-id in voeren.");
        }
    }
}
