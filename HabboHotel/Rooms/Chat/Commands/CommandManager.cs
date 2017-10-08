using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Quasar.Utilities;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

using Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker;
using Quasar.HabboHotel.Rooms.Chat.Commands.Personeel;


using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms.Chat.Commands.Events;
using Quasar.HabboHotel.Items.Wired;
using log4net;

using Quasar.HabboHotel.Global;
namespace Quasar.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";


        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._prefix = Prefix;
            this._commands = new Dictionary<string, IChatCommand>();
            this.RegisterUser();
            this.RegisterModerator();
         }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;
            if (Message == null)
            { }
            else
            {
            }

            if (Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List.Append("Een overzicht van al jouw commands:\n\n");
                foreach (var CmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand(CmdList.Value.PermissionRequired))
                            continue;
                    }
                    List.Append(":" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description + "\n\n");
                }
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return true;
        }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    this.LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId);

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }


                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            this.Register("about", new AboutCommand());
            this.Register("afsluiten", new SluitHotelCommand());
            this.Register("badge", new BadgeSpelerCommand());
            this.Register("ban", new BanSpelerCommand());
            this.Register("banruil", new BanRuilenCommand());
            this.Register("bew", new BewCommand());
            this.Register("bericht", new BerichtSpelerCommand());
            this.Register("bh", new BouwHoogteCommand());
            this.Register("bha", new KamerBerichtCommand());
            this.Register("brb", new BrbCommand());
            this.Register("bubble", new BubbleCommand());
            this.Register("buggs", new BugsCommand());
            this.Register("ca", new CustomizedHotelAlert());
            this.Register("coords", new CoordsCommand());
            this.Register("crackable", new CrackableCommand());
            this.Register("dans", new DansCommand());
            this.Register("dc", new DisconnectSpelerCommand());
            this.Register("draai", new DraaiCommand());
            this.Register("eha", new EventBerichtCommand());
            this.Register("ejectall", new PickallesCommand());
            this.Register("empty", new LeegCommand());
            this.Register("empty_bots", new LeegBotsCommand());
            this.Register("empty_dieren", new LeegDierenCommand());
            this.Register("enable", new EnableCommand());
            this.Register("eventach", new EventAchievementCommand());
            this.Register("faceless", new GezichtloosCommand());
            this.Register("fastfood", new FastFoodCommand());
            this.Register("faq", new FaqCommand());
            this.Register("flagme", new FlagMeCommand());
            this.Register("forceer_flagme", new ForceerFlagmeCommand());
            this.Register("forceer_zeg", new ForceerChatCommand());
            this.Register("forceer_zit", new ForceerZitCommand());
            this.Register("forceer_warp", new ForceerWarpCommand());
            this.Register("forceer_effectenuit", new ForceerEffectenUitCommand());
            this.Register("freeze", new FreezeCommand());
            this.Register("geef", new GeefSpelerCommand());
            this.Register("generatemap", new GenerateMapCommand());
            this.Register("ha", new BerichtHotelCommand());
            this.Register("hal", new BerichtHotelLinkCommand());
            this.Register("handitem", new HandItemCommand());
            this.Register("help", new HelpCommand());
            this.Register("iedereen_kijk", new KijkNaarMijCommand());
            this.Register("iedereen_kom", new KomNaarMijCommand());
            this.Register("inventaris", new CheckInventarisCommand());
            this.Register("ipban", new BanSpelerIPCommand());
            this.Register("item", new UpdateMeubiCommand());
            this.Register("kalender", new KalenderCommand());
            this.Register("kamer", new KamerCommand());
            this.Register("kameralert", new BerichtKamerCommand());
            this.Register("ka", new KamerBerichtCommand());
            this.Register("kamerbadge", new BadgeKamerCommand());
            this.Register("kamerkick", new KickKamerCommand());
            this.Register("kamermute", new KamerMuteCommand());
            this.Register("kamerunmute", new KamerUnmuteCommand());
            this.Register("kick", new KickSpelerCommand());
            this.Register("kickbots", new KickRobotsCommand());
            this.Register("kickdieren", new KickDierenCommand());
      
            this.Register("kus", new KusCommand());
            this.Register("lig", new LigCommand());
            this.Register("massbadge", new BadgeHotelCommand());
            this.Register("massgeef", new MassGiveCommand());
            this.Register("massdans", new MassDansCommand());
            this.Register("massenable", new MassEnableCommand());
            this.Register("mimic", new MimicCommand());
            this.Register("mip", new BanMachineCommand());
            this.Register("moonwalk", new MoonwalkCommand());
            this.Register("mute", new MuteCommand());
            this.Register("mute_bots", new MuteRobotsCommand());
            this.Register("mute_dieren", new MuteDierenCommand());
            this.Register("naarkamer", new NaarKamerCommand());
            
            this.Register("negeer_fluister", new NegeerFluisterCommand());
            this.Register("pickall", new PickallCommand());
            this.Register("poll", new StartQuickPollCommand());
            this.Register("pull", new PullCommand());
            this.Register("push", new PushCommand());
            this.Register("sa", new StaffBerichtCommand());
            this.Register("setmax", new SetmaxCommand());
            this.Register("setspeed", new SetspeedCommand());
            this.Register("sd", new SluitDicesCommand());
            this.Register("spam", new SpamcCommand());
            this.Register("spull", new PullSuperCommand());
            this.Register("spush", new SuperPushCommand());
            this.Register("staan", new StaanCommand());
            this.Register("stats", new StatistiekenCommand());
            this.Register("summon", new SummonCommand());
            this.Register("teleport", new TeleportCommand());
            this.Register("tf", new TransformerenCommand());
            this.Register("ui", new SpelerInformatieCommand());
            this.Register("uit_cadeaus", new UitCadeauCommand());
            this.Register("uit_chatconsole", new UitConsoleChatCommand());
            this.Register("uit_diagonaal", new UitDiagonaalCommand());
            this.Register("uit_fluister", new UitFluisterCommand());
            this.Register("uit_mimic", new UitMimicCommand()); ;
            this.Register("unfreeze", new FreezeUitCommand());
            this.Register("unload", new UnloadCommand());
            this.Register("unmute", new UnmuteCommand());
            this.Register("update", new UpdateCommand());
            this.Register("verwijdergroep", new VerwijderGroepCommand());
            this.Register("volg", new VolgCommand());
            this.Register("warp", new WarpCommand());
            this.Register("welkomsbericht", new WelkomsBerichtCommand());
            this.Register("wisselkoers", new WisselkoersCommand());
            this.Register("zit", new ZitCommand());

        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId)
        {
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", QuasarEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return this._commands.TryGetValue(Command, out IChatCommand);
        }
    }
}
