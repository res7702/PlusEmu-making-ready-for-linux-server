using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.Users;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class KleurCommand : IChatCommand
    {

        public string PermissionRequired
        {
            get { return "command_dnd"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "uit/rood/groen/blauw/paars"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je hebt vergeten een kleur in te vullen..");
                return;
            }
            string chatColour = Params[1];
            string Colour = chatColour.ToLower();
            switch (chatColour)
            {
                case "uit":
                case "zwart":
                case "off":
                    Session.GetHabbo().chatColour = "";
                    Session.SendWhisper("Je chatkleur is gewijzigd naar het origineel!");
                    break;
                case "blue":
                case "red":
                case "cyan":
                case "purple":
                case "green":
                    Session.GetHabbo().chatColour = chatColour;
                    Session.SendWhisper("@"+ Colour +"@Gebruikt nu de kleur " + Colour + "!");
                    break;
                default:
                    Session.SendWhisper("Oeps! De kleur " + Colour + " bestaat helaas niet als chatkleur.");
                    break;
            }
            return;
        }
    }
}