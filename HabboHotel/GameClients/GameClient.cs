using System;

using Quasar.Net;
using Quasar.Core;
using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Interfaces;
using Quasar.HabboHotel.Users.UserDataManagement;

using ConnectionManager;

using Quasar.Communication.Packets.Outgoing.Sound;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Quasar.Communication.Packets.Outgoing.Inventory.Achievements;


using Quasar.Communication.Encryption.Crypto.Prng;
using Quasar.HabboHotel.Users.Messenger.FriendBar;
using Quasar.Communication.Packets.Outgoing.BuildersClub;
using Quasar.HabboHotel.Moderation;

using Quasar.Database.Interfaces;
using Quasar.Utilities;
using Quasar.HabboHotel.Achievements;
using Quasar.HabboHotel.Subscriptions;
using Quasar.HabboHotel.Permissions;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.Communication.Packets.Outgoing.Campaigns;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Nux;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        public ARC4 RC4Client = null;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this._id = ClientId;
            this._connection = pConnection;
            this._packetParser = new GamePacketParser(this);
            this.PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.onNewPacket += parser_onNewPacket;
            byte[] data = (_connection.parser as InitialPacketParser).currentData;
            _connection.parser.Dispose();
            _connection.parser = _packetParser;
            _connection.parser.handlePacketData(data);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                QuasarEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception e)
            {
                Logging.LogPacketException(Message.ToString(), e.ToString());
            }
        }

        internal string SendNotification(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private void PolicyRequest()
        {
            _connection.SendData(QuasarEnvironment.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }


        public void StartConnection()
        {
            if (_connection == null)
                return;

            this.PingCount = 0;

            (_connection.parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.startPacketProcessing();
        }

        internal void SendMessage(string v)
        {
            throw new NotImplementedException();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
                byte errorCode = 0;
                UserData userData = UserDataFactory.GetUserData(AuthTicket, out errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (QuasarEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (QuasarEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (QuasarEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (QuasarEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                QuasarEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;
                if (_habbo != null)
                {
                    userData.user.Init(this, userData);

                    SendMessage(new AuthenticationOKComposer());
                    SendMessage(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    //FurniListNotification -> why?
                    SendMessage(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendMessage(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendMessage(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingAllParts));
                    //1984
                    //2102
                    SendMessage(new UserRightsComposer(_habbo));
                    SendMessage(new AvailabilityStatusComposer());
                    //1044
                    SendMessage(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    //3674
                    //3437
                    SendMessage(new CampaignCalendarDataComposer(_habbo.calendarGift));

                    var habboClubSubscription = new ServerPacket(ServerPacketHeader.HabboClubSubscriptionComposer);
                    habboClubSubscription.WriteString("club_habbo");
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(2);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteBoolean(false);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    habboClubSubscription.WriteInteger(0);
                    SendMessage(habboClubSubscription);

                    SendMessage(new BuildersClubMembershipComposer());
                    SendMessage(new CfhTopicsInitComposer());

                    SendMessage(new BadgeDefinitionsComposer(QuasarEnvironment.GetGame().GetAchievementManager()._achievements));
                    SendMessage(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));  
                    //SendMessage(new TalentTrackLevelComposer());

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (this._habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }

                    PermissionGroup PermissionGroup = null;
                    if (QuasarEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup))
                    {
                        if (!String.IsNullOrEmpty(PermissionGroup.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(PermissionGroup.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(PermissionGroup.Badge, true, this);
                    }

                    SubscriptionData SubData = null;
                    if (QuasarEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(this._habbo.VIPRank, out SubData))
                    {
                        if (!String.IsNullOrEmpty(SubData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(SubData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(SubData.Badge, true, this);
                        }
                    }

                    if (!QuasarEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        QuasarEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    
                    _habbo.InitProcess();

                    if (userData.user.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendMessage(new ModeratorInitComposer(
                          QuasarEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                          QuasarEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                          QuasarEnvironment.GetGame().GetModerationManager().UserActionPresets,
                          QuasarEnvironment.GetGame().GetModerationTool().GetTickets));
                    }
               
                    if (!string.IsNullOrWhiteSpace(QuasarEnvironment.GetDBConfig().DBData["welcome_message"]))
                        SendMessage(new MOTDNotificationComposer(QuasarEnvironment.GetDBConfig().DBData["welcome_message"]));

                    QuasarEnvironment.GetGame().GetRewardManager().CheckRewards(this);

                   
                    if (GetHabbo()._NUX)
                    {
                        var nuxStatus = new ServerPacket(ServerPacketHeader.NuxUserStatus);
                        nuxStatus.WriteInteger(2);
                        SendMessage(nuxStatus);
                         QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("bubble_filter", "Er is een nieuwe gebruiker geregistreerd in Habbis!\n\nGebruiker: " + GetHabbo().Username +"", ""));
                    }

                    if (QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer != null)
                        SendMessage(QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());

                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Bug during user login: " + e);
            }
            return false;
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public int ConnectionID
        {
            get { return _id; }
        }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                   

                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }


            if (!_disconnected)
            {
                if (_connection != null)
                _connection.Dispose();
                _disconnected = true;

                Console.WriteLine("Client has disconnected.");
            }
        }

        public void Dispose()
        {
            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();

            this.MachineId = string.Empty;
            this._disconnected = true;
            this._habbo = null;
            this._connection = null;
            this.RC4Client = null;
            this._packetParser = null;
        }
    }
}