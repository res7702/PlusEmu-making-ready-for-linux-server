#region

using Quasar.HabboHotel.GameClients;

#endregion

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    internal class SetmaxCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_setmax"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Verander het maximum aantal spelers dat in je kamer mogen."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten een (geldig) nummer in te vullen.");
                return;
            }

            int MaxAmount;
            if (int.TryParse(Params[1], out MaxAmount))
            {
                if (MaxAmount == 0)
                {
                    MaxAmount = 10;
                    Session.SendWhisper("Het getal is te laag, het is automatisch naar 10 gezet.");
                }
                else if (MaxAmount > 200 &&
                         !Session.GetHabbo().GetPermissions().HasRight("override_command_setmax_limit"))
                {
                    MaxAmount = 200;
                    Session.SendWhisper("Het getal is te hoog, het is automatisch .");
                }
                else
                    Session.SendWhisper("Aantal spelers die in je kamer mogen verzet naar " + MaxAmount + ".");

                Room.UsersMax = MaxAmount;
                Room.RoomData.UsersMax = MaxAmount;
                using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `rooms` SET `users_max` = " + MaxAmount + " WHERE `id` = '" + Room.Id +
                                      "' LIMIT 1");
                }
            }
            else
                Session.SendWhisper("Oeps, vul een geldig getal in.");
        }
    }
}