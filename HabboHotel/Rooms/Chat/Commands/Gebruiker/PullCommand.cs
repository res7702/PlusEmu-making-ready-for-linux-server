#region

using System;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Quests;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class PullCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_pull"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Trek een gebruiker naar je toe."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten een naam in te vullen!");
                return;
            }

            if (!Room.PullEnabled && !Session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                Session.SendWhisper("Oeps, de kamer eigenaar heeft deze command uitgezet voor de kamer.");
                return;
            }

            var TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper(
                    "De speler kon niet worden gevonden. Waarschijnlijk offline of niet in de kamer aanwezig.");
                return;
            }

            var TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper(
                    "De speler kon niet worden gevonden. Waarschijnljik offline of niet in de kamer aanwezig.");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Je kan deze command niet op jezelf gebruiken.");
                return;
            }

            if (TargetUser.TeleportEnabled)
            {
                Session.SendWhisper("Oeps, deze gebruiker heeft teleporter aan.");
                return;
            }

            var ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (ThisUser.SetX - 1 == Room.GetGameMap().Model.DoorX)
            {
                Session.SendWhisper("Come on man! Je gaat toch niet iemand uit de kamer gooien :(!");
                return;
            }


            var PushDirection = "down";
            if (TargetClient.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId &&
                (Math.Abs(ThisUser.X - TargetUser.X) < 3 && Math.Abs(ThisUser.Y - TargetUser.Y) < 3))
            {
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Trekt " + Params[1] + " naar zich toe*", 0,
                    ThisUser.LastBubble));

                if (ThisUser.RotBody == 0)
                    PushDirection = "up";
                if (ThisUser.RotBody == 2)
                    PushDirection = "right";
                if (ThisUser.RotBody == 4)
                    PushDirection = "down";
                if (ThisUser.RotBody == 6)
                    PushDirection = "left";

                if (PushDirection == "up")
                    TargetUser.MoveTo(ThisUser.X, ThisUser.Y - 1);

                if (PushDirection == "right")
                    TargetUser.MoveTo(ThisUser.X + 1, ThisUser.Y);

                if (PushDirection == "down")
                    TargetUser.MoveTo(ThisUser.X, ThisUser.Y + 1);

                if (PushDirection == "left")
                    TargetUser.MoveTo(ThisUser.X - 1, ThisUser.Y);
            }
            else
            {
                Session.SendWhisper("Je staat te ver weg van deze speler!");
            }
        }
    }
}