using System;
using log4net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Database.Interfaces;

using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class SluitHotelCommand : IChatCommand
    {
        public static readonly ILog log = LogManager.GetLogger("Plus.Core.ConsoleCommandHandler");
        public string PermissionRequired
        {
            get { return "command_sluithotel"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Sluit het hotel af."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            //Logging.DisablePrimaryWriting(true);
            if (Session.GetHabbo().Rank > 7)
            {
                QuasarEnvironment.PerformShutDown();
            }
            else
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
            }
        }
    }
}