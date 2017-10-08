using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class TransformerenCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_transformeren"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Transformeer naar een ander gedaante!"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;

            if (!Room.PetMorphsAllowed)
            {
                Session.SendWhisper("Oeps! Je kan in deze kamer niet transformeren.", 34);
                if (Session.GetHabbo().PetId > 0)
                {
                    Session.SendWhisper("Oeps! Je hebt deze gedaante al.", 34);
                    //Change the users Pet Id.
                    Session.GetHabbo().PetId = 0;

                    //Quickly remove the old user instance.
                    Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));

                    //Add the new one, they won't even notice a thing!!11 8-)
                    Room.SendMessage(new UsersComposer(RoomUser));
                }
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een waarde in te vullen. (Links is een lijst geopend met geldige transformatie alternatieven.)", 34);
                Session.SendMessage(new MassEventComposer("habbopages/transformeren.txt"));
                return;
            }

            if (Params[1].ToString().ToLower() == "lijst")
            {
                Session.SendMessage(new MassEventComposer("habbopages/transformeren.txt"));
                return;
            }

            int TargetPetId = GetPetIdByString(Params[1].ToString());
            if (TargetPetId == 0)
            {
                Session.SendWhisper("Oeps! Je hebt een ongeldige waarde ingevoerd. (Links is een lijst geopend met geldige transformatie alternatieven.)", 34);
                Session.SendMessage(new MassEventComposer("habbopages/transformeren.txt"));
                return;
            }

            //Change the users Pet Id.
            Session.GetHabbo().PetId = (TargetPetId == -1 ? 0 : TargetPetId);

            //Quickly remove the old user instance.
            Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendMessage(new UsersComposer(RoomUser));

            //Tell them a quick message.
            if (Session.GetHabbo().PetId == 60)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_0", "                             Je bent nu een Hond .", ""));
            else if (Session.GetHabbo().PetId == 1)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_1", "                             Je bent nu een Kat.", ""));
            else if (Session.GetHabbo().PetId == 2)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_2", "                             Je bent nu een Terriër.", ""));
            else if (Session.GetHabbo().PetId == 3)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_3", "                             Je bent nu een Krokodil.", ""));
            else if (Session.GetHabbo().PetId == 4)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_4", "                             Je bent nu een Beer.", ""));
            else if (Session.GetHabbo().PetId == 5)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_5", "                             Je bent nu een Varken.", ""));
            else if (Session.GetHabbo().PetId == 6)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_6", "                             Je bent nu een Leeuw.", ""));
            else if (Session.GetHabbo().PetId == 7)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_7", "                             Je bent nu een Neushoorn.", ""));
            else if (Session.GetHabbo().PetId == 8)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_8", "                             Je bent nu een Spin.", ""));
            else if (Session.GetHabbo().PetId == 9)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_9", "                             Je bent nu een Schildpad.", ""));
            else if (Session.GetHabbo().PetId == 10)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_10", "                             Je bent nu een Kuiken.", ""));
            else if (Session.GetHabbo().PetId == 11)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_11", "                             Je bent nu een Kikker.", ""));
            else if (Session.GetHabbo().PetId == 12)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_12", "                             Je bent nu een Draak.", ""));
            else if (Session.GetHabbo().PetId == 13)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_13", "                             Je bent nu de Slendermen.", ""));
            else if (Session.GetHabbo().PetId == 14)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_14", "                             Je bent nu een Aap.", ""));
            else if (Session.GetHabbo().PetId == 15)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_15", "                             Je bent nu een Paard.", ""));
            else if (Session.GetHabbo().PetId == 16)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_16", "                             Je bent nu een Monsterplant.", ""));
            else if (Session.GetHabbo().PetId == 17)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_17", "                             Je bent nu een Goedaardig Konijn.", ""));
            else if (Session.GetHabbo().PetId == 18)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_18", "                             Je bent nu een Slechtaardig Konijn.", ""));
            else if (Session.GetHabbo().PetId == 19)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_19", "                             Je bent nu een Depressief Konijn.", ""));
            else if (Session.GetHabbo().PetId == 20)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_20", "                             Je bent nu een Zachtaardig Konijn.", ""));
            else if (Session.GetHabbo().PetId == 21)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_21", "                             Je bent nu een Goedaardige Duif.", ""));
            else if (Session.GetHabbo().PetId == 22)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_22", "                             Je bent nu een Slechtaardige Duif.", ""));
            else if (Session.GetHabbo().PetId == 23)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_23", "                             Je bent nu een Demoon Aap.", ""));
            else if (Session.GetHabbo().PetId == 24)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_24", "                             Je bent nu een Beren Pup.", ""));
            else if (Session.GetHabbo().PetId == 25)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_25", "                             Je bent nu een Terriër Pup.", ""));
            else if (Session.GetHabbo().PetId == 26)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_26", "                             Je bent nu een Dwerg.", ""));
            else if (Session.GetHabbo().PetId == 27)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_27", "                             Je bent nu een Baby.", ""));
            else if (Session.GetHabbo().PetId == 28)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_28", "                             Je bent nu een Kitten.", ""));
            else if (Session.GetHabbo().PetId == 29)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_29", "                             Je bent nu een Honden Pup.", ""));
            else if (Session.GetHabbo().PetId == 30)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_30", "                             Je bent nu een Varken Pup.", ""));
            else if (Session.GetHabbo().PetId == 31)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_31", "                             Je bent nu een Oempa Loempa.", ""));
            else if (Session.GetHabbo().PetId == 32)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_32", "                             Je bent nu een Steen.", ""));
            else if (Session.GetHabbo().PetId == 33)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_33", "                             Je bent nu een Pterodactylus.", ""));
            else if (Session.GetHabbo().PetId == 34)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_34", "                             Je bent nu een Velociraptor.", ""));
            else if (Session.GetHabbo().PetId == 35)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_35", "                             Je bent nu een Wolf.", ""));
            else if (Session.GetHabbo().PetId == 36)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_36", "                             Je bent nu een Monster Konijn.", ""));
            else if (Session.GetHabbo().PetId == 37)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_37", "                             Je bent nu Pickachu.", ""));
            else if (Session.GetHabbo().PetId == 38)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_38", "                             Je bent nu een Pinguin.", ""));
            else if (Session.GetHabbo().PetId == 39)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_39", "                             Je bent nu Mario.", ""));
            else if (Session.GetHabbo().PetId == 40)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_40", "                             Je bent nu een Olifant.", ""));
            else if (Session.GetHabbo().PetId == 41)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_41", "                             Je bent nu een Spookachtig Konijn.", ""));
            else if (Session.GetHabbo().PetId == 42)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_42", "                             Je bent nu een Goudkleurig Konijn.", ""));
            else if (Session.GetHabbo().PetId == 43)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_43", "                             Je bent nu een Roze Mewtwo.", ""));
            else if (Session.GetHabbo().PetId == 44)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_44", "                             Je bent nu een Entei.", ""));
            else if (Session.GetHabbo().PetId == 45)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_45", "                             Je bent nu een Blauwe Mewtwo.", ""));
            else if (Session.GetHabbo().PetId == 46)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_46", "                             Je bent nu een Cavia.", ""));
            else if (Session.GetHabbo().PetId == 47)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_47", "                             Je bent nu een Uil.", ""));
            else if (Session.GetHabbo().PetId == 48)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_48", "                             Je bent nu een Goude Mewtwo.", ""));
            else if (Session.GetHabbo().PetId == 49)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_49", "                             Je bent nu een Eend.", ""));
            else if (Session.GetHabbo().PetId == 50)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_50", "                             Je bent nu een Baby.", ""));
            else if (Session.GetHabbo().PetId == 51)
                Session.SendMessage(RoomNotificationComposer.SendBubble("bubble_transformeren_51", "                             Je bent nu een Baby.", ""));
        }

        private int GetPetIdByString(string Pet)
        {
            switch (Pet.ToLower())
            {
                default:
                    return 0;
                case "terug":
                case "habbis":
                case "-1":
                    return -1;
                case "0":
                    return 60;//This should be 0.
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                    return 3;
                case "4":
                    return 4;
                case "5":
                    return 5;
                case "6":
                    return 6;
                case "7":
                    return 7;
                case "8":
                    return 8;
                case "9":
                    return 9;
                case "10":
                    return 10;
                case "11":
                    return 11;
                case "12":
                    return 12;
                case "13":
                    return 13;
                case "14":
                    return 14;
                case "15":
                    return 15;
                case "16":
                    return 16;
                case "17":
                    return 17;
                case "18":
                    return 18;
                case "19":
                    return 19;
                case "20":
                    return 20;
                case "21":
                    return 21;
                case "22":
                    return 22;
                case "23":
                    return 23;
                case "24":
                    return 24;
                case "25":
                    return 25;
                case "26":
                    return 26;
                case "27":
                    return 27;
                case "28":
                    return 28;
                case "29":
                    return 29;
                case "30":
                    return 30;
                case "31":
                    return 31;
                case "32":
                    return 32;
                case "33":
                    return 33;
                case "34":
                    return 34;
                case "35":
                    return 35;
                case "36":
                    return 36;
                case "37":
                    return 37;
                case "38":
                    return 38;
                case "39":
                    return 39;
                case "40":
                    return 40;
                case "41":
                    return 41;
                case "42":
                    return 42;
                case "43":
                    return 43;
                case "44":
                    return 44;
                case "45":
                    return 45;
                case "46":
                    return 46;
                case "47":
                    return 47;
                case "48":
                    return 48;
                case "49":
                    return 49;
                case "50":
                    return 50;
                case "51":
                    return 51;
            }
        }
    }
}