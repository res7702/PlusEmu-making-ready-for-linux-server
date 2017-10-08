using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;


using log4net;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Global
{
    static class LanguageLocale
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Global.LanguageLocale");

        private static Dictionary<string, Language> Languages { get; set; }

        public static void Init()
        {

            Languages = new Dictionary<string, Language>();

            Languages.Add("en", new Global.Language("en"));
            Languages.Add("nl", new Global.Language("nl"));


            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Console.WriteLine(CurrentTime + "» Hotel Vertalingen");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(CurrentTime + "» Hotel » Geladen!");
            Console.ResetColor();
        }

        public static string Get(GameClient Session, string Local)
        {

            string Language = "en";
            if(Session?.GetHabbo() != null)
            {
                Language = Session.GetHabbo().Language;
            }

            return Get(Language, Local);
        }

        public static string Get(string Language, string Local)
        {
            if(!Languages.ContainsKey(Language))
            {
                return "unkown_language";
            }

            return Languages[Language].Get(Local);
        }
    }
}