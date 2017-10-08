using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class KamerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kamer"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Bekijk de functies die je in je kamer kan wijzigen.."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een functie in te voeren. (:lijst)", 34);
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                return;
            }

            string Option = Params[1];
            switch (Option)
            {
                case "lijst":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Lijst met kamer functies:");
                        List.AppendLine("-------------------------");
                        List.AppendLine("Huisdieren: " + (Room.PetMorphsAllowed == true ? "aan" : "uit"));
                        List.AppendLine("Pull: " + (Room.PullEnabled == true ? "aan" : "uit"));
                        List.AppendLine("Push: " + (Room.PushEnabled == true ? "aan" : "uit"));
                        List.AppendLine("Super Pull: " + (Room.SPullEnabled == true ? "aan" : "uit"));
                        List.AppendLine("Super Push: " + (Room.SPushEnabled == true ? "aan" : "uit"));
                        List.AppendLine("Respect: " + (Room.RespectNotificationsEnabled == true ? "aan" : "uit"));
                        List.AppendLine("Effects: " + (Room.EnablesEnabled == true ? "aan" : "uit"));
                        Session.SendNotification(List.ToString());
                        break;
                    }

                case "push":
                    {
                        Room.PushEnabled = !Room.PushEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", QuasarEnvironment.BoolToEnum(Room.PushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.PushEnabled == true ? "weer andere Habbis duwen!" : "niet meer andere Habbis duwen!"), 34);
                        break;
                    }

                case "spush":
                    {
                        Room.SPushEnabled = !Room.SPushEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", QuasarEnvironment.BoolToEnum(Room.SPushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.SPushEnabled == true ? "weer andere Habbis super duwen!" : "niet meer andere Habbis super duwen!"), 34);
                        break;
                    }

                case "spull":
                    {
                        Room.SPullEnabled = !Room.SPullEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", QuasarEnvironment.BoolToEnum(Room.SPullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.SPullEnabled == true ? "weer andere Habbis super trekken!" : "niet meer andere Habbis super trekken!"), 34);
                        break;
                    }

                case "pull":
                    {
                        Room.PullEnabled = !Room.PullEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", QuasarEnvironment.BoolToEnum(Room.PullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.PullEnabled == true ? "weer andere Habbis trekken!" : "niet meer andere Habbis trekken!"), 34);
                        break;
                    }

                case "enable":
                case "effecten":
                    {
                        Room.EnablesEnabled = !Room.EnablesEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("EnablesEnabled", QuasarEnvironment.BoolToEnum(Room.EnablesEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.EnablesEnabled == true ? "weer effecten gebruiken!" : "geen effecten meer gebruiken!"), 34);
                        break;
                    }

               
                case "respect":
                    {
                        Room.RespectNotificationsEnabled = !Room.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", QuasarEnvironment.BoolToEnum(Room.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.RespectNotificationsEnabled == true ? "weer andere Habbis respecteren!" : "niet meer andere Habbis respecteren!"), 34);
                        break;
                    }

                case "dieren":
                case "huisdieren":
                    {
                        Room.PetMorphsAllowed = !Room.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", QuasarEnvironment.BoolToEnum(Room.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Habbis kunnen nu " + (Room.PetMorphsAllowed == true ? "weer veranderen in een huisdier!" : "niet meer veranderen in een huisdier!"), 34);

                        if (!Room.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                    continue;

                                User.GetClient().SendWhisper("De kamer eigenaar heeft deze actie voor de kamer uitgezet.", 34);
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
                                    //Tell the user what is going on.
                                    User.GetClient().SendWhisper("Oeps! Je kan deze actie hier niet uitvoeren.", 34);

                                    //Change the users Pet Id.
                                    User.GetClient().GetHabbo().PetId = 0;

                                    //Quickly remove the old user instance.
                                    Room.SendMessage(new UserRemoveComposer(User.VirtualId));

                                    //Add the new one, they won't even notice a thing!!11 8-)
                                    Room.SendMessage(new UsersComposer(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
