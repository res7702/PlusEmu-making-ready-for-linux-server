#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class VolgCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_volg"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Volg een andere gebruiker."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten een gebruikernaam in te voeren..");
                return;
            }

            var TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper(
                    "De speler kon niet worden gevonden. Waarschijnljik offline of niet in een kamer aanwezig.");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom == Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Misschien een bril nodig? " + TargetClient.GetHabbo().Username +
                                    " is bij jou in de kamer!");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Je kan jezelf niet volgen..");
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Deze gebruiker is op dit moment in het hotel overzicht.");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom.Access != RoomAccess.OPEN &&
                !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("Oeps, de gebruiker die je wilt volgen is in een gesloten kamer.");
                return;
            }

            Session.GetHabbo().PrepareRoom(TargetClient.GetHabbo().CurrentRoom.RoomId, "");
        }
    }
}