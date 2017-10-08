using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class MassGiveCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mass_geef"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Geef iedereen in het hotel Credits, Diamanten of Duckets."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je hebt geen waarde ingevoerd [Credits, Diamanten of Duckets].");
                return;
            }
            
            string UpdateVal = Params[1];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_credits"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                foreach (GameClient Target in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().Credits = Target.GetHabbo().Credits += Amount;
                                    Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Credit(s) ontvangen van het Hotel Management!");
                                    Session.SendWhisper("Je hebt " + Amount + " Credit(s) gegeven aan iedereen die online is.", 34);

                                }

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
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                foreach (GameClient Target in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().Duckets += Amount;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, Amount));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Ducket(s) ontvangen van het Hotel Management!");
                                    Session.SendWhisper("Je hebt " + Amount + " Ducket(s) gegeven aan " + Target.GetHabbo().Username + " die online is.", 34);
                                }
                                
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
                case "diamonds":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_diamanten"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                foreach (GameClient Target in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().Diamonds += Amount;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, Amount, 5));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                    Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Diamant(en) ontvangen van het Hotel Management!");
                                    Session.SendWhisper("Je hebt " + Amount + " Diamant(en) gegeven aan iedereen die online is.", 34);
                                }
                                
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oeps! Het ingevulde bedrag is niet geldig.", 34);
                                break;
                            }
                        }
                    }

                case "punten":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_geef_punten"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                            {
                                if (Amount > 50)
                                {
                                    Session.SendWhisper("Je kunt niet meer dan 50 punten per keer sturen.");
                                    return;
                                }

                                foreach (GameClient Target in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                                {
                                    if (Target == null || Target.GetHabbo() == null || Target.GetHabbo().Username == Session.GetHabbo().Username)
                                        continue;

                                    Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + Amount;
                                    Target.GetHabbo().UserPoints = Target.GetHabbo().UserPoints + 1;
                                    Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, Amount, 103));

                                    if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                        Target.SendNotification("Je hebt <b>" + Amount.ToString() + "</b> Punt(en) ontvangen van het Hotel Management!");
                                        Session.SendWhisper("Je hebt " + Amount + " Punt(en) gegeven aan iedereen die online is.", 34);
                                }

                                
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
