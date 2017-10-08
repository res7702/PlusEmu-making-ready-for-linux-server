using Quasar.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    internal class StartQuickPollCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een vraag in te voeren.");
            }
            else
            {

                string quest = CommandManager.MergeParams(Params, 1);
                if (quest == "stop")
                {
                    Room.endQuestion();
                }
                else
                {
                    Room.startQuestion(quest);
                }

            }
        }

        public string Description =>
            "Start/stop een kamer poll.";

        public string Parameters =>
            "";

        public string PermissionRequired =>
            "command_poll_start";
    }
}