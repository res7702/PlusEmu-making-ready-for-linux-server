using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class GeefSpelerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_geef"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef een gebruiker Diamanten, Duckets of Credits."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("", 34);
                return;
            }

            GameClient Target = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oeps! Je bent vergeten een gebruiker in te vullen.", 34);
                return;
            }

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_credits"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                int cred = Target.GetHabbo().Credits += Amount;
                                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                Target.SendMessage(new CreditBalanceComposer(cred));

                                //if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Credit(s) ontvangen van " + Session.GetHabbo().Username + "!");
                                Session.SendWhisper("Je hebt " + Amount + " Credit(s) gegeven aan " + Target.GetHabbo().Username + "!", 34);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oeps! Het ingevulde bedrag is niet geldig.", 34);
                                break;
                            }
                        }
                    }

                case "pixels":
                case "duckets":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_duckets"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Duckets += Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));

                                //if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Ducket(s) ontvangen van " + Session.GetHabbo().Username + "!");
                                Session.SendWhisper("Je hebt " + Amount + " Ducket(s) gegeven aan " + Target.GetHabbo().Username + "!", 34);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oeps! Het ingevulde bedrag is niet geldig.", 34);
                                break;
                            }
                        }
                    }

                case "belcredits":
                case "diamanten":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_diamanten"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().Diamonds += Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Amount, 5));

                                //if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Diamant(en) ontvangen van " + Session.GetHabbo().Username + "!");
                                Session.SendWhisper("Je hebt " + Amount + " Diamant(en) gegeven aan " + Target.GetHabbo().Username + "!", 34);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oeps! Het ingevulde bedrag is niet geldig.", 34);
                                break;
                            }
                        }
                    }

                case "op":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_op"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.", 34);
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                                //if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Punt(en) ontvangen van " + Session.GetHabbo().Username + "!");
                                Session.SendWhisper("Je hebt " + Amount + " Punt(en) gegeven aan " + Target.GetHabbo().Username + "!", 34);
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oeps! Het ingevulde bedrag is niet geldig.", 34);
                                break;
                            }
                        }
                    }
                default:
                    Session.SendWhisper("'" + UpdateVal + "' is geen geldige waarde!", 34);
                    break;
            }
        }
    }
}