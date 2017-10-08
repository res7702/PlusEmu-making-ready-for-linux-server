using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class ForceerWarpCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_forceer_warp"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Teleporteer een speler naar je toe."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Room.CheckRights(Session) || User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je moet wel een gebruikersnaam invullen.", 34);
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps! Deze gebruiker is niet in de kamer aanwezig, of is niet online.", 34);
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Oeps!Deze gebruiker is niet in de kamer aanwezig, of is niet online.", 34);
                return;
            }


            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (TargetUser == ThisUser)
            {
                Session.SendWhisper("Oeps! Jezelf teleporteren naar deze plek gaat niet. Je staat er toch al gekkie!", 34);
                return;
            }

            TargetUser.TeleportEnabled = !TargetUser.TeleportEnabled; //Enable TP
            Room.GetGameMap().GenerateMaps();

            /*if (ThisUser.RotBody % 2 != 0)
                ThisUser.RotBody--;
            if (ThisUser.RotBody == 0)
                TargetUser.MoveTo(ThisUser.X, ThisUser.Y - 1);
            else if (ThisUser.RotBody == 2)
                TargetUser.MoveTo(ThisUser.X + 1, ThisUser.Y);
            else if (ThisUser.RotBody == 4)
                TargetUser.MoveTo(ThisUser.X, ThisUser.Y + 1);
            else if (ThisUser.RotBody == 6)
                TargetUser.MoveTo(ThisUser.X - 1, ThisUser.Y);*/
            TargetUser.MoveTo(ThisUser.X, ThisUser.Y);

            TargetUser.TeleportEnabled = !TargetUser.TeleportEnabled; //Disable TP
            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("Je hebt " + TargetClient.GetHabbo().Username + " laten teleporteren.", 34);
            TargetClient.SendWhisper(Session.GetHabbo().Username + "Je bent geteleporteerd door de eigenaar van de kamer.", 34);
        }
    }
}
