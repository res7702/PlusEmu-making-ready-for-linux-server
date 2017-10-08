using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Global
{
    class Language
    {
        public string Location { get; private set; }

        public Dictionary<string, string> Locals { get; private set; }

        public bool IsValid { get; private set; }

        public Language(string Location)
        {
            this.Locals = new Dictionary<string, string>();
            this.IsValid = false;
            string FileName = $"language/{Location}.lang";

            if (!File.Exists(FileName))
            {
                Console.WriteLine($"[Error][{Location}] Error loading language file");
                return;
            }

            foreach (string Line in File.ReadAllLines(FileName))
            {
                string[] Parts = Line.Split('=');
                if(Parts.Length != 2)
                {
                    Console.WriteLine($"[Error][{Location}] Error parsing language local: {Line}");
                    continue;
                }

                this.Locals.Add(Parts[0].Trim().ToLower(),
                    Parts[1]);
            }

        }

        public string Get(string Local)
        {
            Local = Local.ToLower();
            return (this.IsValid && this.Locals.ContainsKey(Local) ? this.Locals[Local] : $"unknown_{Local}");
        }
    }
}
