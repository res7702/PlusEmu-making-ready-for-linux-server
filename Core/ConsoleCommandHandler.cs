using System;
using log4net;
using Quasar.HabboHotel;
using Quasar.HabboHotel.Global;

using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.Core
{
    public class ConsoleCommandHandler
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.Core.ConsoleCommandHandler");

        public static object LanguageLocal { get; private set; }

        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return;

            try
            {
                #region Command parsing
                string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region stop
                    case "stop":
                    case "shutdown":
                        {
                            Logging.DisablePrimaryWriting(true);
                            Logging.WriteLine("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!", ConsoleColor.Yellow);
                            QuasarEnvironment.PerformShutDown();
                            break;
                        }
                    #endregion

                    #region alert
                    case "alert":
                        {
                            string Notice = inputData.Substring(6);

                            QuasarEnvironment.GetGame().GetClientManager().SendMessage(new BroadcastMessageAlertComposer(Notice));

                            Console.WriteLine(CurrentTime + "» Alert met succes verzonden");
                            break;
                        }
                    #endregion

                    default:
                        {
                            Console.WriteLine(CurrentTime + "» " + parameters[0].ToLower() + " is een niet bestaande functie - toets :help in voor meer informatie.");
                            break;
                        }
                }
                #endregion
            }
            catch (Exception e)
            {
                string CurrentTime1 = DateTime.Now.ToString("HH:mm:ss" + " | ");
                Console.WriteLine(CurrentTime1 + "» Fout in command [" + inputData + "]: " + e);
            }
        }
    }
}