using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Core;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Rooms.TraxMachine;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class UpdateCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_update"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Update een bepaald onderdeel van 't hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Je bent vergeten om een onderdeel in te typen.");
                return;
            }


            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
               
                #region Lijst
                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine("Update de volgende onderdelen doormiddel van de command:");
                        List.AppendLine(":update catalog - Update de Habbis shop.");
                        List.AppendLine(":update items - Update al de items.");
                        List.AppendLine(":update models - Update kamer models.");
                        List.AppendLine(":update filter - Update de woordfilter.");
                        List.AppendLine(":update chars - Update de karakters replacement van de woordfilter.");
                        List.AppendLine(":update navigator - Update de navigator.");
                        List.AppendLine(":update rights - Update permissions & ranks.");
                        List.AppendLine(":update config - Update belangrijke instellingen van 't hotel.");
                        List.AppendLine(":update bans - Update de verbanningen.");
                        List.AppendLine(":update tickets - Update de Moderator Tickets.");
                        List.AppendLine(":update badges - Update alles rondom badges.");
                        List.AppendLine(":update aanbiedingen - Update de aanbiedingen.");
                        Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                        break;
                    }
                #endregion

                #region Catalogus
                case "cata":
                case "catalog":
                case "catalogue":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_cata"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        QuasarEnvironment.GetGame().getCatalogFrontPage().Init();
                        QuasarEnvironment.GetGame().GetCatalog().Init(QuasarEnvironment.GetGame().GetItemManager());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Het onderdeel 'Catalogus' is succesvol geüpdatet.", "catalog/open/" + Message + "test"));

                        break;
                    }

                #endregion

                #region Pinatas
                case "pinatas":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_pinatas"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetPinataManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Het onderdeel 'Pinatas' is succesvol geüpdatet.", ""));
                        break;
                    }
                #endregion

                #region Woordfilter Karakters
                case "chars":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitCharacters();
                        Session.SendWhisper("Het onderdeel 'Woordfilter Replacement Karakters' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Items
                case "items":
                case "furni":
                case "furniture":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furniture"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetItemManager().Init();
                        Session.SendWhisper("Het onderdeel 'Items' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Models
                case "models":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_models"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetRoomManager().LoadModels();
                        Session.SendWhisper("Het onderdeel 'Kamer Models' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Promoties
                case "promoties":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_promoties"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetLandingManager().LoadPromotions();
                        Session.SendWhisper("Het onderdeel 'Promoties' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Youtube
                case "youtube":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_youtube"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetTelevisionManager().Init();
                        Session.SendWhisper("Het onderdeel 'Youtube Televisies' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Aanbiedingen
                case "aanbiedingen":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                    {
                        Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                        break;
                    }

                    QuasarEnvironment.GetGame().GetTargetedOffersManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                    Session.SendWhisper("Het onderdeel 'Aanbiedingen' is succesvol geüpdatet.");
                    break;
                #endregion

                #region Filter
                case "filter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitWords();
                        Session.SendWhisper("Het onderdeel 'Woordfilter' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Navigator
                case "navigator":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_navigator"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetNavigator().Init();
                        Session.SendWhisper("Het onderdeel 'Navigator' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Ranks
                case "ranks":
                case "rights":
                case "permissions":
                case "commands":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_ranks"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetPermissionManager().Init();

                        foreach (GameClient Client in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                                continue;

                            Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }

                        Session.SendWhisper("Het onderdeel 'Ranks en Rechten' is succesvol geüpdatet.");
                        break;
                    }

                #endregion

                #region Configuratie
                case "config":
                case "settings":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuratie"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.ConfigData = new ConfigData();
                        Session.SendWhisper("Het onderdeel 'Settings' is succesvol geüpdatet.");
                        break;
                    }

                #endregion

                #region Bans
                case "bans":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bans"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetModerationManager().ReCacheBans();
                        Session.SendWhisper("Het onderdeel 'Bans' is succesvol geüpdatet.");
                        break;
                    }

                #endregion

                #region Quests
                case "quests":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_quests"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetQuestManager().Init();
                        Session.SendWhisper("Het onderdeel 'Quests' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Achievements
                case "achievements":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_achievements"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetAchievementManager().LoadAchievements();
                        Session.SendWhisper("Het onderdeel 'Achievements' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Moderation
                case "moderation":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_moderation"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetModerationManager().Init();
                        QuasarEnvironment.GetGame().GetClientManager().ModAlert("Personeel opgelet: Er zijn wat wijzigingen aan de Moderator Tools aangebracht. Relog het hotel om de veranderingen te bekijken.");

                        Session.SendWhisper("Het onderdeel 'Moderator Tools' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Tickets
                case "tickets":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_tickets"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren."); ;
                            break;
                        }

                        if (QuasarEnvironment.GetGame().GetModerationTool().Tickets.Count > 0)
                            QuasarEnvironment.GetGame().GetModerationTool().Tickets.Clear();

                        QuasarEnvironment.GetGame().GetClientManager().ModAlert("Personeel opgelet: Er zijn wat wijzigingen aan de Moderator Tools aangebracht. Relog het hotel om de veranderingen te bekijken.");

                        Session.SendWhisper("Het onderdeel 'Tickets' is succesvol geüpdatet.");
                        break;
                    }

                #endregion

                #region Vouchers
                case "vouchers":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_vouchers"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetCatalog().GetVoucherManager().Init();
                        Session.SendWhisper("Het onderdeel 'Vouchers' is succesvol geüpdatet.");
                        break;
                    }

                #endregion

                #region Gamecenter
                case "gc":
                case "games":
                case "gamecenter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_gamecenter"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetGameDataManager().Init();
                        Session.SendWhisper("Het onderdeel 'Gamecenter' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Huisdieren
                case "huisdieren":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_huisdieren"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetPetLocale().Init();
                        Session.SendWhisper("Het onderdeel 'Huisdieren' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Jukebox
                case "jukebox":
               
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_cata"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }
                        int count = TraxSoundManager.Songs.Count;
                        TraxSoundManager.Init();
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Het onderdeel 'Catalogus' is succesvol geüpdatet."));
                        break;
                    }
                #endregion

                #region Mutant
                case "mutant":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_antimutant"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetAntiMutant().Init();
                        Session.SendWhisper("Het onderdeel 'Anti-Mutant' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Bots
                case "bots":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bots"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetBotManager().Init();
                        Session.SendWhisper("Het onderdeel 'Bots' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Rewards
                case "rewards":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetRewardManager().Reload();
                        Session.SendWhisper("Het onderdeel 'Rewards' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Chat Styles
                case "chat_styles":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_chatstyles"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetChatStyles().Init();
                        Session.SendWhisper("Het onderdeel 'Chatstyles' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                #region Badges
                case "badges":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_badges"))
                        {
                            Session.SendWhisper("Oeps! Je hebt niet de bevoegdheid om deze actie uit te voeren.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetBadgeManager().Init();
                        Session.SendWhisper("Het onderdeel 'Badges' is succesvol geüpdatet.");
                        break;
                    }
                #endregion

                 default:
                    Session.SendWhisper("Oeps! Het onderdeel '" + UpdateVariable + "' bestaat niet.");
                    break;
            }
        }
    }
}
