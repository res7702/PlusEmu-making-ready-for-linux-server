using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class BerichtHotelLinkCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_berichtlinkhotel"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Stuur een bericht naar al de online gebruikers (met link)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een bericht in te vullen.", 34);
                return;
            }

            string URL = Params[1];

            string Message = CommandManager.MergeParams(Params, 2);
            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("Hotel Management", "Een bericht van <b>" + Session.GetHabbo().Username + "</b>.\n\nKlik op de link om naar een andere website te gaan.\n\n" + "<b>Website details: </b>" + Message + "\n\n", "hal", "Ga naar website!", URL));
           return;
        }
    }
}
