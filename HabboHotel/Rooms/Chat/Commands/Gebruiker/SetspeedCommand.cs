#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class SetspeedCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_setspeed"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Versnel of vertraag de snelheid van rollers in je kamer."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten aan te geven welke snelheid (getal) je wilt.");
                return;
            }

            int Speed;
            if (int.TryParse(Params[1], out Speed))
            {
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetSpeed(Speed);
            }
            else
                Session.SendWhisper("Vul een geldig getal in.");
        }
    }
}