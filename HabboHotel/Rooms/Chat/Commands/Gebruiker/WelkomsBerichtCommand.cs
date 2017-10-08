using Quasar.Communication.Packets.Outgoing.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Events
{
    internal class WelkomsBerichtCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_welkomsbericht";
            }
        }
        public string Parameters
        {
            get
            {
                return "";
            }
        }
        public string Description
        {
            get
            {
                return "Lees nogmaals het welkomsbericht van het hotel.";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!string.IsNullOrWhiteSpace(QuasarEnvironment.GetDBConfig().DBData["welcome_message"]))
                Session.SendMessage(new MOTDNotificationComposer(QuasarEnvironment.GetDBConfig().DBData["welcome_message"]));

        }
    }
}