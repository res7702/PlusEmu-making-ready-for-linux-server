#region

using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Users;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class FlagMeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_flagme"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Deze command geef je de optie om je naam te veranderen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!CanChangeName(Session.GetHabbo()))
            {
                Session.SendWhisper("Oeps, het lijkt erop dat je op dit moment niet je naam kan veranderen.");
                return;
            }

            Session.GetHabbo().ChangingName = true;
            Session.SendNotification(
                "Let op dat wanneer je naam ongepast is kan je worden verbannen.\r\rKlik dit bericht weg en klik op jezelf om te beginnen!");
            Session.SendMessage(new UserObjectComposer(Session.GetHabbo()));
            
        }

        private bool CanChangeName(Habbo Habbo)
        {
            if (Habbo.Rank == 1 && Habbo.VIPRank == 0 && Habbo.LastNameChange == 0)
                return true;
            if (Habbo.Rank == 1 && Habbo.VIPRank == 1 &&
                (Habbo.LastNameChange == 0 || (QuasarEnvironment.GetUnixTimestamp() + 604800) > Habbo.LastNameChange))
                return true;
            if (Habbo.Rank == 1 && Habbo.VIPRank == 2 &&
                (Habbo.LastNameChange == 0 || (QuasarEnvironment.GetUnixTimestamp() + 86400) > Habbo.LastNameChange))
                return true;
            if (Habbo.Rank == 1 && Habbo.VIPRank == 3)
                return true;
            if (Habbo.GetPermissions().HasRight("mod_tool"))
                return true;

            return false;
        }
    }
}