using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using log4net;
using Quasar.Core;
using System.Text;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Badges;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.HabboHotel.Camera;
using Quasar.Communication.Packets.Outgoing.Rooms.Camera;
using Quasar.Communication.Packets.Incoming.Rooms.Camera;
using Quasar.HabboHotel.Users;

namespace Quasar.Messages.Net
{

    public class MusConnection
    {
        private Socket _socket;
        private byte[] _buffer = new byte[1024];

        private static readonly ILog log = LogManager.GetLogger("Quasar.Messages.Net.MusConnection");

        public MusConnection(Socket Socket)
        {
            this._socket = Socket;

            try
            {
                this._socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnEvent_RecieveData, _socket);
            }
            catch { this.tryClose(); }
        }

        public void tryClose()
        {
            try
            {
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.Close();
                this._socket.Dispose();
            }
            catch
            {
            }

            this._socket = null;
            this._buffer = null;
        }

        public void OnEvent_RecieveData(IAsyncResult iAr)
        {
            try
            {
                int bytes = 0;

                try
                {
                    bytes = _socket.EndReceive(iAr);
                }
                catch
                {
                    tryClose();
                    return;
                }

                String data = Encoding.Default.GetString(_buffer, 0, bytes);

                if (data.Length > 0)
                    processCommand(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            tryClose();
        }

        public void processCommand(String data)
        {
            GameClient Client = null;

            String header = data.Split(Convert.ToChar(1))[0];
            String param = data.Split(Convert.ToChar(1))[1];

            string[] Params = param.ToString().Split(':');
            int UserId, RoomId = 0;
            switch (header.ToLower())
            {
                #region User Related
                #region :reload_credits <UserID>
                case "reload_credits":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        int Credits = 0;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `credits` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", UserId);
                            Credits = dbClient.getInteger();
                        }

                        Client.GetHabbo().Credits = Credits;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        break;
                    }
                #endregion
                #region :reload_pixels <UserID>
                case "reload_pixels":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        int Pixels = 0;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `activity_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", UserId);
                            Pixels = dbClient.getInteger();
                        }

                        Client.GetHabbo().Duckets = Pixels;
                        Client.SendMessage(new HabboActivityPointNotificationComposer(Client.GetHabbo().Duckets, Pixels));
                        break;
                    }
                #endregion
                #region :reload_diamonds <UserID>
                case "reload_diamonds":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        int Diamonds = 0;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `vip_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", UserId);
                            Diamonds = dbClient.getInteger();
                        }

                        Client.GetHabbo().Diamonds = Diamonds;
                        Client.SendMessage(new HabboActivityPointNotificationComposer(Diamonds, 0, 5));
                        break;
                    }
                #endregion
                #region :reload_gotw <UserID>
                case "reload_gotw":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        int GOTWPoints = 0;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `gotw_points` FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", UserId);
                            GOTWPoints = dbClient.getInteger();
                        }

                        Client.GetHabbo().GOTWPoints = GOTWPoints;
                        Client.SendMessage(new HabboActivityPointNotificationComposer(GOTWPoints, 0, 103));
                        break;
                    }
                #endregion
                case "moveto":
                    {
                        UserId = Convert.ToInt32(Params[0]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        Room Testing = Client?.GetHabbo()?.CurrentRoom;
                        RoomUser Testing2;
                        if((Testing2 = Testing?.GetRoomUserManager()?.GetRoomUserByHabbo(Client.GetHabbo().Username)) != null)
                        {
                            Testing2.MoveTo(12, 12);
                        }
                        break;
                    }
                #region :reload_user_rank userID
                case "reload_user_rank":
                    {
                        UserId = Convert.ToInt32(Params[0]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `rank` FROM `users` WHERE `id` = @userID LIMIT 1");
                            dbClient.AddParameter("userID", UserId);
                            Client.GetHabbo().Rank = dbClient.getInteger();
                        }
                        break;
                    }
                #endregion
                #region :reload_user_vip userID
                case "reload_user_vip":
                    {
                        UserId = Convert.ToInt32(Params[0]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `rank_vip` FROM `users` WHERE `id` = @userID LIMIT 1");
                            dbClient.AddParameter("userID", UserId);
                            Client.GetHabbo().VIPRank = dbClient.getInteger();
                        }
                        break;
                    }
                #endregion
                #region :reload_motto userID
                case "reload_motto":
                    {
                        UserId = Convert.ToInt32(Params[0]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `motto` FROM `users` WHERE `id` = @userID LIMIT 1");
                            dbClient.AddParameter("userID", UserId);
                            Client.GetHabbo().Motto = dbClient.getString();
                        }

                        if (Client.GetHabbo().InRoom)
                        {
                            Room Room = Client.GetHabbo().CurrentRoom;
                            if (Room == null)
                                return;

                            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                            if (User == null || User.GetClient() == null)
                                return;

                            Room.SendMessage(new UserChangeComposer(User, false));
                        }
                        break;
                    }
                #endregion
                #region :alert_user <userid> <message>
                case "alert":
                case "alert_user":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        string alertMessage = Convert.ToString(Params[1]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        Client.SendMessage(new BroadcastMessageAlertComposer(alertMessage));
                        break;
                    }
                #endregion
                #region :reload_badges <UserID>
                case "update_badges":
                case "reload_badges":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);

                        if (Client != null)
                        {
                            if (Client.GetHabbo() != null)
                            {
                                Client.SendMessage(new BadgesComposer(Client));
                            }
                        }
                        break;
                    }
                #endregion
                #region :givebadge <UserID> <badge>
                case "givebadge":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        string badgeCode = Convert.ToString(Params[1]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);

                        if (Client != null)
                        {
                            if (Client.GetHabbo() != null)
                            {
                                Client.GetHabbo().GetBadgeComponent().GiveBadge(badgeCode, true, Client);
                            }
                        }
                        break;
                    }
                #endregion
                #region :disconnect <username>
                case "disconnect":
                    {
                        try
                        {
                            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(Params[0]));
                            if (TargetClient != null && TargetClient.GetConnection() != null)
                                TargetClient.GetConnection().Dispose();
                        }
                        catch
                        {
                            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                            Console.WriteLine(CurrentTime + "» Fout tijdens disconnecten van een gebruiker via MUS");
                        }
                        return;
                    }
                #endregion
                #region :reload_last_change userID
                case "reload_last_change":
                    {
                        UserId = Convert.ToInt32(Params[0]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `last_change` FROM `users` WHERE `id` = @userID LIMIT 1");
                            dbClient.AddParameter("userID", UserId);
                            Client.GetHabbo().LastNameChange = dbClient.getInteger();
                        }
                        break;
                    }
                #endregion
                #region :goto <UserID> <RoomID>
                case "goto":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                       RoomId = Convert.ToInt32(Params[1]);

                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        if (!int.TryParse(Params[1], out RoomId))
                            break;
                        else
                        {
                            Room _room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                            if (_room == null)
                                Client.SendNotification("Failed to find the requested room!");
                            else
                            {
                                if (!Client.GetHabbo().InRoom)
                                    Client.SendMessage(new RoomForwardComposer(_room.Id));
                                else
                                    Client.GetHabbo().PrepareRoom(_room.Id, "");
                            }
                        }
                    }
                    break;
                #endregion
                #endregion

                #region Fastfood
                #region :progress_achievement
                case "progress_achievement":
                    {
                        UserId = Convert.ToInt32(Params[0]);
                        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                        if (Client == null || Client.GetHabbo() == null)
                            break;

                        string Achievement = Convert.ToString(Params[1]);
                        int Progress = Convert.ToInt32(Params[2]);

                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Client, Achievement, Progress);
                        break;
                    }
                #endregion
                #endregion

                #region Settings related
                #region :reload_filter/:update_filter
                case "update_filter":
                case "reload_filter":
                case "recache_filter":
                case "refresh_filter":
                    {
                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitWords();
                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitCharacters();
                        break;
                    }
                #endregion
                #region :reload_catalog/:reload_catalog
                case "update_catalog":
                case "reload_catalog":
                case "recache_catalog":
                case "refresh_catalog":
                case "update_catalogue":
                case "reload_catalogue":
                case "recache_catalogue":
                case "refresh_catalogue":
                    {
                        QuasarEnvironment.GetGame().GetCatalog().Init(QuasarEnvironment.GetGame().GetItemManager());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        break;
                    }
                #endregion
                #region :reload_items/:update_items
                case "update_items":
                case "reload_items":
                case "recache_items":
                case "refresh_items":
                    {
                        QuasarEnvironment.GetGame().GetItemManager().Init();
                        break;
                    }
                #endregion
                #region :reload_navigator/:update_navigator
                case "update_navigator":
                case "reload_navigator":
                case "recache_navigator":
                case "refresh_navigator":
                    {
                        QuasarEnvironment.GetGame().GetNavigator().Init();
                        break;
                    }
                #endregion
                #region :reload_ranks/:update_ranks
                case "update_ranks":
                case "reload_ranks":
                case "recache_ranks":
                case "refresh_ranks":
                    {
                        QuasarEnvironment.GetGame().GetPermissionManager().Init();

                        foreach (GameClient C in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (C == null || C.GetHabbo() == null || C.GetHabbo().GetPermissions() == null)
                                continue;

                            C.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }
                        break;
                    }
                #endregion
                #region :reload_settings/:update_settings
                case "update_settings":
                case "reload_settings":
                case "recache_settings":
                case "refresh_settings":
                    {
                        QuasarEnvironment.ConfigData = new ConfigData();
                        break;
                    }
                #endregion
                #region :reload_quests/:update_quests
                case "reload_quests":
                case "update_quests":
                    {
                        QuasarEnvironment.GetGame().GetQuestManager().Init();
                        break;
                    }
                #endregion
                #region :reload_vouchers/:update_vouchers
                case "reload_vouchers":
                case "update_vouchers":
                    {
                        QuasarEnvironment.GetGame().GetCatalog().GetVoucherManager().Init();
                        break;
                    }
                #endregion
                #region :reload_bans/:update_bans
                case "update_bans":
                case "reload_bans":
                    {
                        QuasarEnvironment.GetGame().GetModerationManager().ReCacheBans();
                        break;
                    }
                #endregion
                #endregion

             
                //#region Camera related

                //    #region :add_preview <photo_id> <user_id> <created_at>
                //    case "add_preview":
                //    {
                //        int PhotoId = Convert.ToInt32(Params[0]);
                //        UserId = Convert.ToInt32(Params[1]);
                //        long CreatedAt = Convert.ToInt64(Params[2]);

                //        Client = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);

                //                    if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().CurrentRoomId < 1)
                //                           break;

                //        QuasarEnvironment.GetGame().GetCameraManager().AddPreview(new CameraPhotoPreview(PhotoId, UserId, CreatedAt));
                //                    break;
                //    }

                //    #endregion

                //    #endregion

                default:
                    {
                        string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                        Console.WriteLine(CurrentTime + "» Onbekende Mus-pakket: '" + header + "'");
                        return;
                    }
            }
            string CurrentTime1 = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Console.WriteLine(CurrentTime1 + "» Mus-command gestuurd: '" + header + "'");
        }
    }
}