using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using log4net;

using Quasar.Core;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Achievements;
using Quasar.HabboHotel.Users.Badges;
using Quasar.HabboHotel.Users.Inventory;
using Quasar.HabboHotel.Users.Messenger;
using Quasar.HabboHotel.Users.Relationships;
using Quasar.HabboHotel.Users.UserDataManagement;

using Quasar.HabboHotel.Users.Process;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;


using Quasar.HabboHotel.Users.Navigator.SavedSearches;
using Quasar.HabboHotel.Users.Effects;
using Quasar.HabboHotel.Users.Messenger.FriendBar;
using Quasar.HabboHotel.Users.Clothing;
using Quasar.Communication.Packets.Outgoing.Navigator;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.Communication.Packets.Outgoing.Handshake;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Rooms.Chat.Commands;
using Quasar.HabboHotel.Users.Permissions;
using Quasar.HabboHotel.Subscriptions;
using Quasar.HabboHotel.Club;
using System.Linq;

namespace Quasar.HabboHotel.Users
{
    public class Habbo
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Users");

        public string Language { get; set; } = "nl"; //change to load dynamically from usuers table later...

        //Generic player values.
        private int _id;
        private string _username;
        private int _rank;
        private string _motto;
        private string _look;
        private string _gender;
        private string _footballLook;
        private string _footballGender;
        private string _backupLook;
        private bool _lastMovFGate;
        private string _backupGender;
        private int _credits;
        private int _duckets;
        private int _diamonds;
        private int _pumpkins;
        private int _gotwPoints;
        private int _userpoints;
        private int _homeRoom;
        private double _lastOnline;
        private double _accountCreated;
        private List<int> _clientVolume;
        private double _lastNameChange;
        private string _machineId;
        private bool _chatPreference;
        private bool _focusPreference;
        private bool _isExpert;
        private int _vipRank;
        private int _CurrentTalentLevel;
      

        //Abilitys triggered by generic events.

        private bool _appearOffline;
        private bool _allowTradingRequests;
        private bool _allowUserFollowing;
        private bool _allowFriendRequests;
        private bool _allowMessengerInvites;
        private bool _allowPetSpeech;
        private bool _allowBotSpeech;
        private bool _allowPublicRoomStatus;
        private bool _allowConsoleMessages;
        private bool _allowGifts;
        private bool _allowMimic;
        private bool _receiveWhispers;
        private bool _ignorePublicWhispers;
        private bool _playingFastFood;
        private FriendBarState _friendbarState;
        private int _christmasDay;
        private int _wantsToRideHorse;
        private int _timeAFK;
        private bool _disableForcedEffects;

        public long _lastTimeUsedHelpCommand;
        public long _lastTimeKissedSomeone;
        private bool _ViewInventory;

        //Player saving.
        private bool _disconnected;
        private bool _habboSaved;
        private bool _changingName;

        //Counters
        private double _floodTime;
        private int _friendCount;
        private double _timeMuted;
        private double _tradingLockExpiry;
        private int _bannedPhraseCount;
        private double _sessionStart;
        private int _messengerSpamCount;
        private double _messengerSpamTime;
        private int _creditsTickUpdate;
        public byte _guidelevel;
        public byte _hulptroepen;

        //Room related
        private int _tentId;
        private int _hopperId;
        private bool _isHopping;
        private int _teleportId;
        private bool _isTeleporting;
        private int _teleportingRoomId;
        private bool _roomAuthOk;
        private int _currentRoomId;

        //Advertising reporting system.
        private bool _hasSpoken;
        private bool _advertisingReported;
        private double _lastAdvertiseReport;
        private bool _advertisingReportBlocked;

        //Values generated within the game.
        private bool _wiredInteraction;
        private int _questLastCompleted;
        private bool _inventoryAlert;
        private bool _ignoreBobbaFilter;
        private bool _wiredTeleporting;
        private int _customBubbleId;
        private int _tempInt;
        private bool _onHelperDuty;

        /// <summary>
        /// Player to update, handle, change etc.
        /// </summary>

        // Camara

        
        public string lastPhotoPreview;
        public string _lastPhotoPreview;

        //Fastfood
        private int _fastfoodScore;

        // Calendar
        public bool[] calendarGift;

        //Just random fun stuff.
        private int _petId;

        //Anti-script placeholders.
        private DateTime _lastGiftPurchaseTime;
        private DateTime _lastMottoUpdateTime;
        private DateTime _lastClothingUpdateTime;
        private DateTime _lastPhotoPurchaseTime; // Scripting Foto

        public int LastSqlQuery = 0;
        private DateTime _lastForumMessageUpdateTime;

        private int _giftPurchasingWarnings;
        private int _mottoUpdateWarnings;
        private int _clothingUpdateWarnings;

        public bool _NUX;
        public bool PassedNuxNavigator = false, PassedNuxDuckets = false, PassedNuxItems = false, PassedNuxChat = false, PassedNuxCatalog = false;

        private bool _sessionGiftBlocked;
        private bool _sessionMottoBlocked;
        private bool _sessionClothingBlocked;


        public List<int> RatedRooms;
        public List<int> MutedUsers;
        public List<RoomData> UsersRooms;

        private GameClient _client;
        private HabboStats _habboStats;
        private HabboMessenger Messenger;
        private ClubManager ClubManager;
        private ProcessComponent _process;
        public ArrayList FavoriteRooms;
        public Dictionary<int, int> quests;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        public Dictionary<int, Relationship> Relationships;
        public ConcurrentDictionary<string, UserAchievement> Achievements;

        private DateTime _timeCached;

        private SearchesComponent _navigatorSearches;
        private EffectsComponent _fx;
        private ClothingComponent _clothing;
        private PermissionComponent _permissions;

        private IChatCommand _iChatCommand;
        private Dictionary<int, UserTalent> _Talents;
        public bool IsCitizen => CurrentTalentLevel > 4;

        public int CameraCount { get; set; }

        internal string chatColour;
        
             public Habbo(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits, int ActivityPoints, int HomeRoom,
            bool HasFriendRequestsDisabled, int LastOnline, bool AppearOffline, bool HideInRoom, double CreateDate, int Diamonds,
            string machineID, string clientVolume, bool ChatPreference, bool FocusPreference, bool PetsMuted, bool BotsMuted, bool AdvertisingReportBlocked, double LastNameChange,
            int GOTWPoints, int UserPoints, bool IgnoreInvites, double TimeMuted, double TradingLock, bool AllowGifts, int FriendBarState, bool DisableForcedEffects, 
            bool AllowMimic, int VIPRank, byte guidelevel, byte hulptroepen, bool Nux, int Pumpkins, int CameraCount)
        {
            this.CameraCount = CameraCount;
            this._id = Id;
            this._username = Username;
            this._rank = Rank;
            this._motto = Motto;
            this._look = QuasarEnvironment.GetGame().GetAntiMutant().RunLook(Look);
            this._gender = Gender.ToLower();
            this._footballLook = QuasarEnvironment.FilterFigure(Look.ToLower());
            this._footballGender = Gender.ToLower();
            this._credits = Credits;
            this._duckets = ActivityPoints;
            this._diamonds = Diamonds;
            this._gotwPoints = GOTWPoints;
            this._pumpkins = Pumpkins;
            this._NUX = Nux;
            this._userpoints = UserPoints;
            this._homeRoom = HomeRoom;
            this._lastOnline = LastOnline;
            this._guidelevel = guidelevel;
            this._hulptroepen = hulptroepen;
            this._accountCreated = CreateDate;
            this._clientVolume = new List<int>();
            this._Talents = new Dictionary<int, UserTalent>();
            foreach (string Str in clientVolume.Split(','))
            {
                int Val = 0;
                if (int.TryParse(Str, out Val))
                    this._clientVolume.Add(int.Parse(Str));
                else
                    this._clientVolume.Add(100);
            }

            this._lastNameChange = LastNameChange;
            this._machineId = machineID;
            this._chatPreference = ChatPreference;
            this._focusPreference = FocusPreference;
            this._isExpert = IsExpert == true;

            this._appearOffline = AppearOffline;
            this._allowTradingRequests = true;//TODO
            this._allowUserFollowing = true;//TODO
            this._allowFriendRequests = HasFriendRequestsDisabled;//TODO
            this._allowMessengerInvites = IgnoreInvites;
            this._allowPetSpeech = PetsMuted;
            this._allowBotSpeech = BotsMuted;
            this._allowPublicRoomStatus = HideInRoom;
            this._allowConsoleMessages = true;
            this._allowGifts = AllowGifts;
            this._allowMimic = AllowMimic;
            this._lastPhotoPreview = lastPhotoPreview;
            this._receiveWhispers = true;
            this._ignorePublicWhispers = false;
            this._playingFastFood = false;
            this._friendbarState = FriendBarStateUtility.GetEnum(FriendBarState);
            this._christmasDay = ChristmasDay;
            this._wantsToRideHorse = 0;
            this._timeAFK = 0;
            this._disableForcedEffects = DisableForcedEffects;
            this._vipRank = VIPRank;

            this._disconnected = false;
            this._habboSaved = false;
            this._changingName = false;

            this._floodTime = 0;
            this._friendCount = 0;
            this._timeMuted = TimeMuted;
            this._timeCached = DateTime.Now;

            this._CurrentTalentLevel = GetCurrentTalentLevel();

            this._tradingLockExpiry = TradingLock;
            if (this._tradingLockExpiry > 0 && QuasarEnvironment.GetUnixTimestamp() > this.TradingLockExpiry)
            {
                this._tradingLockExpiry = 0;
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Id + "' LIMIT 1");
                }
            }

            this._bannedPhraseCount = 0;
            this._sessionStart = QuasarEnvironment.GetUnixTimestamp();
            this._messengerSpamCount = 0;
            this._messengerSpamTime = 0;
            this._creditsTickUpdate = QuasarStaticGameSettings.UserCreditsUpdateTimer;

            this._tentId = 0;
            this._hopperId = 0;
            this._isHopping = false;
            this._teleportId = 0;
            this._isTeleporting = false;
            this._teleportingRoomId = 0;
            this._roomAuthOk = false;
            this._currentRoomId = 0;

            this._hasSpoken = false;
            this._lastAdvertiseReport = 0;
            this._advertisingReported = false;
            this._advertisingReportBlocked = AdvertisingReportBlocked;

            this._wiredInteraction = false;
            this._questLastCompleted = 0;
            this._inventoryAlert = false;
            this._ignoreBobbaFilter = false;
            this._wiredTeleporting = false;
            this._customBubbleId = 0;
            this._onHelperDuty = false;
            this._fastfoodScore = 0;
            this._petId = 0;
            this._tempInt = 0;

            this._lastGiftPurchaseTime = DateTime.Now;
            this._lastMottoUpdateTime = DateTime.Now;
            this._lastPhotoPurchaseTime = DateTime.Now; // Photo Scripting


            this._lastClothingUpdateTime = DateTime.Now;
            this._lastForumMessageUpdateTime = DateTime.Now;

            this._giftPurchasingWarnings = 0;
            this._mottoUpdateWarnings = 0;
            this._clothingUpdateWarnings = 0;

            this._sessionGiftBlocked = false;
            this._sessionMottoBlocked = false;
            this._sessionClothingBlocked = false;

            this.FavoriteRooms = new ArrayList();
            this.MutedUsers = new List<int>();
            this.Achievements = new ConcurrentDictionary<string, UserAchievement>();
            this.Relationships = new Dictionary<int, Relationship>();
            this.RatedRooms = new List<int>();
            this.UsersRooms = new List<RoomData>();
            this._ViewInventory = false;

            //TODO: Nope.
            this.InitPermissions();

            #region Stats
            DataRow StatRow = null;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                StatRow = dbClient.getRow();

                if (StatRow == null)//No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `user_stats` (`id`) VALUES ('" + Id + "')");
                    dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", Id);
                    StatRow = dbClient.getRow();
                }

                try
                {
                    this._habboStats = new HabboStats(Convert.ToInt32(StatRow["roomvisits"]), Convert.ToDouble(StatRow["onlineTime"]), Convert.ToInt32(StatRow["respect"]), Convert.ToInt32(StatRow["respectGiven"]), Convert.ToInt32(StatRow["giftsGiven"]),
                        Convert.ToInt32(StatRow["giftsReceived"]), Convert.ToInt32(StatRow["dailyRespectPoints"]), Convert.ToInt32(StatRow["dailyPetRespectPoints"]), Convert.ToInt32(StatRow["AchievementScore"]),
                        Convert.ToInt32(StatRow["quest_id"]), Convert.ToInt32(StatRow["quest_progress"]), Convert.ToInt32(StatRow["groupid"]), Convert.ToString(StatRow["respectsTimestamp"]), Convert.ToInt32(StatRow["forum_posts"]));

                    if (Convert.ToString(StatRow["respectsTimestamp"]) != DateTime.Today.ToString("MM/dd"))
                    {
                        _habboStats.RespectsTimestamp = DateTime.Today.ToString("MM/dd");

                        var DailyRespects = 3;

                        _habboStats.DailyRespectPoints = DailyRespects;
                        _habboStats.DailyPetRespectPoints = DailyRespects;

                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + DailyRespects +
                                          "', `dailyPetRespectPoints` = '" + DailyRespects +
                                          "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") +
                                          "' WHERE `id` = '" + Id + "' LIMIT 1");
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e.ToString());
                }
            }

            Group G = null;
            if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(this._habboStats.FavouriteGroupId, out G))
                this._habboStats.FavouriteGroupId = 0;
            #endregion
        }



        internal ClubManager GetClubManager()
        {
            return this.ClubManager;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }

        public int Rank
        {
            get { return this._rank; }
            set { this._rank = value; }
        }

        public string Motto
        {
            get { return this._motto; }
            set { this._motto = value; }
        }

        public string Look
        {
            get { return this._look; }
            set { this._look = value; }
        }

        public string Gender
        {
            get { return this._gender; }
            set { this._gender = value; }
        }

        public string FootballLook
        {
            get { return this._footballLook; }
            set { this._footballLook = value; }
        }

        public string FootballGender
        {
            get { return this._footballGender; }
            set { this._footballGender = value; }
        }

        public bool LastMovFGate
        {
            get { return this._lastMovFGate; }
            set { this._lastMovFGate = value; }
        }

        public string BackupLook
        {
            get { return this._backupLook; }
            set { this._backupLook = value; }
        }

        public string BackupGender
        {
            get { return this._backupGender; }
            set { this._backupGender = value; }
        }

        public int Credits
        {
            get { return this._credits; }
            set { this._credits = value; }
        }

        public int Duckets
        {
            get { return this._duckets; }
            set { this._duckets = value; }
        }

        public bool ViewInventory
        {
            get { return this._ViewInventory; }
            set { this._ViewInventory = value; }
        }

        public int Diamonds
        {
            get { return this._diamonds; }
            set { this._diamonds = value; }
        }

        public int GOTWPoints
        {
            get { return this._gotwPoints; }
            set { this._gotwPoints = value; }
        }
       
        public int Pumpkins
        {
            get { return this._pumpkins; }
            set { this._pumpkins = value; }
        }
       
        public int UserPoints
        {
            get { return this._userpoints; }
            set { this._userpoints = value; }
        }

        public int HomeRoom
        {
            get { return this._homeRoom; }
            set { this._homeRoom = value; }
        }

        public double LastOnline
        {
            get { return this._lastOnline; }
            set { this._lastOnline = value; }
        }

        public double AccountCreated
        {
            get { return this._accountCreated; }
            set { this._accountCreated = value; }
        }

        public List<int> ClientVolume
        {
            get { return this._clientVolume; }
            set { this._clientVolume = value; }
        }

        public double LastNameChange
        {
            get { return this._lastNameChange; }
            set { this._lastNameChange = value; }
        }

        public string MachineId
        {
            get { return this._machineId; }
            set { this._machineId = value; }
        }

        public bool ChatPreference
        {
            get { return this._chatPreference; }
            set { this._chatPreference = value; }
        }
        public bool FocusPreference
        {
            get { return this._focusPreference; }
            set { this._focusPreference = value; }
        }

        public bool IsExpert
        {
            get { return this._isExpert; }
            set { this._isExpert = value; }
        }

        public bool AppearOffline
        {
            get { return this._appearOffline; }
            set { this._appearOffline = value; }
        }

        public int VIPRank
        {
            get { return this._vipRank; }
            set { this._vipRank = value; }
        }

        public int TempInt
        {
            get { return this._tempInt; }
            set { this._tempInt = value; }
        }

        public bool AllowTradingRequests
        {
            get { return this._allowTradingRequests; }
            set { this._allowTradingRequests = value; }
        }

        public bool AllowUserFollowing
        {
            get { return this._allowUserFollowing; }
            set { this._allowUserFollowing = value; }
        }

        public bool AllowFriendRequests
        {
            get { return this._allowFriendRequests; }
            set { this._allowFriendRequests = value; }
        }

        public bool AllowMessengerInvites
        {
            get { return this._allowMessengerInvites; }
            set { this._allowMessengerInvites = value; }
        }

        public bool AllowPetSpeech
        {
            get { return this._allowPetSpeech; }
            set { this._allowPetSpeech = value; }
        }

        public bool AllowBotSpeech
        {
            get { return this._allowBotSpeech; }
            set { this._allowBotSpeech = value; }
        }

        public bool AllowPublicRoomStatus
        {
            get { return this._allowPublicRoomStatus; }
            set { this._allowPublicRoomStatus = value; }
        }

        public bool AllowConsoleMessages
        {
            get { return this._allowConsoleMessages; }
            set { this._allowConsoleMessages = value; }
        }

        public bool AllowGifts
        {
            get { return this._allowGifts; }
            set { this._allowGifts = value; }
        }

        public bool AllowMimic
        {
            get { return this._allowMimic; }
            set { this._allowMimic = value; }
        }

        public bool ReceiveWhispers
        {
            get { return this._receiveWhispers; }
            set { this._receiveWhispers = value; }
        }

        public bool IgnorePublicWhispers
        {
            get { return this._ignorePublicWhispers; }
            set { this._ignorePublicWhispers = value; }
        }

        public bool PlayingFastFood
        {
            get { return this._playingFastFood; }
            set { this._playingFastFood = value; }
        }

        public FriendBarState FriendbarState
        {
            get { return this._friendbarState; }
            set { this._friendbarState = value; }
        }

        public int ChristmasDay
        {
            get { return this._christmasDay; }
            set { this._christmasDay = value; }
        }

        public int WantsToRideHorse
        {
            get { return this._wantsToRideHorse; }
            set { this._wantsToRideHorse = value; }
        }

        public int TimeAFK
        {
            get { return this._timeAFK; }
            set { this._timeAFK = value; }
        }

        public bool DisableForcedEffects
        {
            get { return this._disableForcedEffects; }
            set { this._disableForcedEffects = value; }
        }

        public bool ChangingName
        {
            get { return this._changingName; }
            set { this._changingName = value; }
        }

        public int FriendCount
        {
            get { return this._friendCount; }
            set { this._friendCount = value; }
        }

        public double FloodTime
        {
            get { return this._floodTime; }
            set { this._floodTime = value; }
        }

        public int BannedPhraseCount
        {
            get { return this._bannedPhraseCount; }
            set { this._bannedPhraseCount = value; }
        }

        public bool RoomAuthOk
        {
            get { return this._roomAuthOk; }
            set { this._roomAuthOk = value; }
        }

        public int CurrentRoomId
        {
            get { return this._currentRoomId; }
            set { this._currentRoomId = value; }
        }

        public int QuestLastCompleted
        {
            get { return this._questLastCompleted; }
            set { this._questLastCompleted = value; }
        }

        public int MessengerSpamCount
        {
            get { return this._messengerSpamCount; }
            set { this._messengerSpamCount = value; }
        }

        public double MessengerSpamTime
        {
            get { return this._messengerSpamTime; }
            set { this._messengerSpamTime = value; }
        }

        public double TimeMuted
        {
            get { return this._timeMuted; }
            set { this._timeMuted = value; }
        }

        public double TradingLockExpiry
        {
            get { return this._tradingLockExpiry; }
            set { this._tradingLockExpiry = value; }
        }

        public double SessionStart
        {
            get { return this._sessionStart; }
            set { this._sessionStart = value; }
        }

        public int TentId
        {
            get { return this._tentId; }
            set { this._tentId = value; }
        }

        public int HopperId
        {
            get { return this._hopperId; }
            set { this._hopperId = value; }
        }

        public bool IsHopping
        {
            get { return this._isHopping; }
            set { this._isHopping = value; }
        }

        public int TeleporterId
        {
            get { return this._teleportId; }
            set { this._teleportId = value; }
        }

        public bool IsTeleporting
        {
            get { return this._isTeleporting; }
            set { this._isTeleporting = value; }
        }

        public int TeleportingRoomID
        {
            get { return this._teleportingRoomId; }
            set { this._teleportingRoomId = value; }
        }

        public bool HasSpoken
        {
            get { return this._hasSpoken; }
            set { this._hasSpoken = value; }
        }

        public double LastAdvertiseReport
        {
            get { return this._lastAdvertiseReport; }
            set { this._lastAdvertiseReport = value; }
        }

        public bool AdvertisingReported
        {
            get { return this._advertisingReported; }
            set { this._advertisingReported = value; }
        }

        public bool AdvertisingReportedBlocked
        {
            get { return this._advertisingReportBlocked; }
            set { this._advertisingReportBlocked = value; }
        }

        public bool WiredInteraction
        {
            get { return this._wiredInteraction; }
            set { this._wiredInteraction = value; }
        }

        public bool InventoryAlert
        {
            get { return this._inventoryAlert; }
            set { this._inventoryAlert = value; }
        }

        public bool IgnoreBobbaFilter
        {
            get { return this._ignoreBobbaFilter; }
            set { this._ignoreBobbaFilter = value; }
        }

        public bool WiredTeleporting
        {
            get { return this._wiredTeleporting; }
            set { this._wiredTeleporting = value; }
        }

        public int CustomBubbleId
        {
            get { return this._customBubbleId; }
            set { this._customBubbleId = value; }
        }

        public bool OnHelperDuty
        {
            get { return this._onHelperDuty; }
            set { this._onHelperDuty = value; }
        }

        public int FastfoodScore
        {
            get { return this._fastfoodScore; }
            set { this._fastfoodScore = value; }
        }

        public int PetId
        {
            get { return this._petId; }
            set { this._petId = value; }
        }

        public int CreditsUpdateTick
        {
            get { return this._creditsTickUpdate; }
            set { this._creditsTickUpdate = value; }
        }

        public IChatCommand IChatCommand
        {
            get { return this._iChatCommand; }
            set { this._iChatCommand = value; }
        }

        public DateTime LastGiftPurchaseTime
        {
            get { return this._lastGiftPurchaseTime; }
            set { this._lastGiftPurchaseTime = value; }
        }

        public DateTime LastPhotoPurchaseTime
        {
            get { return this._lastPhotoPurchaseTime; }
            set { this._lastPhotoPurchaseTime = value; }
        }

        public DateTime LastMottoUpdateTime
        {
            get { return this._lastMottoUpdateTime; }
            set { this._lastMottoUpdateTime = value; }
        }

        public DateTime LastClothingUpdateTime
        {
            get { return this._lastClothingUpdateTime; }
            set { this._lastClothingUpdateTime = value; }
        }

        public DateTime LastForumMessageUpdateTime
        {
            get { return this._lastForumMessageUpdateTime; }
            set { this._lastForumMessageUpdateTime = value; }
        }

        // raul
        public UserAchievement GetOrAddAchievementData(string k)
        {
            UserAchievement ach;
            if (!Achievements.TryGetValue(k, out ach))
            {
                ach = new UserAchievement(k, 0, 0);
                Achievements.TryAdd(k, ach);
            }

            return ach;
        }

        public int GiftPurchasingWarnings
        {
            get { return this._giftPurchasingWarnings; }
            set { this._giftPurchasingWarnings = value; }
        }

        public int MottoUpdateWarnings
        {
            get { return this._mottoUpdateWarnings; }
            set { this._mottoUpdateWarnings = value; }
        }

        public int ClothingUpdateWarnings
        {
            get { return this._clothingUpdateWarnings; }
            set { this._clothingUpdateWarnings = value; }
        }

        public Dictionary<int, UserTalent> Talents
        {
            get { return this._Talents; }
            set { this._Talents = value; }
        }

        public int CurrentTalentLevel
        {
            get { return this._CurrentTalentLevel; }
            set { this._CurrentTalentLevel = value; }
        }

        public bool SessionGiftBlocked
        {
            get { return this._sessionGiftBlocked; }
            set { this._sessionGiftBlocked = value; }
        }

        public bool SessionMottoBlocked
        {
            get { return this._sessionMottoBlocked; }
            set { this._sessionMottoBlocked = value; }
        }

        public bool SessionClothingBlocked
        {
            get { return this._sessionClothingBlocked; }
            set { this._sessionClothingBlocked = value; }
        }

        public HabboStats GetStats()
        {
            return this._habboStats;
        }

        public bool InRoom
        {
            get
            {
                return CurrentRoomId >= 1 && CurrentRoom != null;
            }
        }

        public Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                    return null;

                Room _room = null;
                if (QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(CurrentRoomId, out _room))
                    return _room;

                return null;
            }
        }

        public bool CacheExpired()
        {
            TimeSpan Span = DateTime.Now - _timeCached;
            return (Span.TotalMinutes >= 30);
        }

        public string GetQueryString
        {
            get
            {
                this._habboSaved = true;
                return "UPDATE `users` SET `online` = '0', `last_online` = '" + QuasarEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + this.Duckets + "', `credits` = '" + this.Credits + "', `vip_points` = '" + this.Diamonds + "', `home_room` = '" + this.HomeRoom + "', `gotw_points` = '" + this.GOTWPoints + "', `user_points` = '" + this.UserPoints + "', `time_muted` = '" + this.TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(this._friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + this._habboStats.RoomVisits + "', `onlineTime` = '" + (QuasarEnvironment.GetUnixTimestamp() - SessionStart + this._habboStats.OnlineTime) + "', `respect` = '" + this._habboStats.Respect + "', `respectGiven` = '" + this._habboStats.RespectGiven + "', `giftsGiven` = '" + this._habboStats.GiftsGiven + "', `giftsReceived` = '" + this._habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + this._habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + this._habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + this._habboStats.AchievementPoints + "', `quest_id` = '" + this._habboStats.QuestID + "', `quest_progress` = '" + this._habboStats.QuestProgress + "', `groupid` = '" + this._habboStats.FavouriteGroupId + "',`forum_posts` = '" + this._habboStats.ForumPosts + "' WHERE `id` = '" + this.Id + "' LIMIT 1;";
            }
        }

        public bool InitProcess()
        {
            this._process = new ProcessComponent();
            if (this._process.Init(this))
                return true;
            return false;
        }

        public void InitCalendar()
        {
            if (!QuasarEnvironment.GetGame().GetCalendarManager().CampaignEnable())
                return;

            calendarGift = new bool[QuasarEnvironment.GetGame().GetCalendarManager().GetTotalDays()];
            for (int i = 0; i < calendarGift.Length; i++)
                calendarGift[i] = false;

            DataTable dTable = null;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT day FROM user_campaign_gifts WHERE user_id = '" + Id + "' AND campaign_name = @name");
                dbClient.AddParameter("name", QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignName());
                dTable = dbClient.getTable();
            }

            if (dTable != null)
            {
                foreach (DataRow dRow in dTable.Rows)
                {
                    int Day = (int)dRow["day"];
                    calendarGift[Day - 1] = true;
                }
            }
        }

        public bool InitSearches()
        {
            this._navigatorSearches = new SearchesComponent();
            if (this._navigatorSearches.Init(this))
                return true;
            return false;
        }

        public bool InitFX()
        {
            this._fx = new EffectsComponent();
            if (this._fx.Init(this))
                return true;
            return false;
        }

        public bool InitClothing()
        {
            this._clothing = new ClothingComponent();
            if (this._clothing.Init(this))
                return true;
            return false;
        }

        private bool InitPermissions()
        {
            this._permissions = new PermissionComponent();
            if (this._permissions.Init(this))
                return true;
            return false;
        }

        public void LoadTalents(Dictionary<int, UserTalent> talents)
        {
            this._Talents = talents;
        }

        public UserTalent GetTalentData(int t)
        {
            UserTalent result;
            this._Talents.TryGetValue(t, out result);

            return result;
        }

        public int GetCurrentTalentLevel()
        {
            int level = this._Talents.Values.Select(current => QuasarEnvironment.GetGame().GetTalentManager().GetTalent(current.TalentId).Level).Concat(new[] { 1 }).Max();
            return level;
        }

        public void InitInformation(UserData data)
        {
            BadgeComponent = new BadgeComponent(this, data);
            Relationships = data.Relations;
        }

        public void Init(GameClient client, UserData data)
        {
            this.Achievements = data.achievements;

            this.FavoriteRooms = new ArrayList();
            foreach (int id in data.favouritedRooms)
            {
                FavoriteRooms.Add(id);
            }

            this.MutedUsers = data.ignores;

            this._client = client;
            BadgeComponent = new BadgeComponent(this, data);
            InventoryComponent = new InventoryComponent(Id, client);

            quests = data.quests;

            Messenger = new HabboMessenger(Id);
            Messenger.Init(data.friends, data.requests);
            this._friendCount = Convert.ToInt32(data.friends.Count);
            this._disconnected = false;
            UsersRooms = data.rooms;
            Relationships = data.Relations;

            this.InitSearches();
            this.InitFX();
            this.InitClothing();
            this.LoadTalents(data.Talents);
            this.ClubManager = new ClubManager(this.Id, data);
            this.InitCalendar();

        }


        public PermissionComponent GetPermissions()
        {
            return this._permissions;
        }

        public void OnDisconnect()
        {
            if (this._disconnected)
                return;

            try
            {
                if (this._process != null)
                    this._process.Dispose();
            }
            catch { }

            this._disconnected = true;

            if (this.ClubManager != null)
            {
                this.ClubManager.Clear();
                this.ClubManager = (ClubManager)null;
            }

            QuasarEnvironment.GetGame().GetClientManager().UnregisterClient(Id, Username);

            if (!this._habboSaved)
            {
                this._habboSaved = true;
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.runFastQuery("UPDATE `users` SET `online` = '0', `last_online` = '" + QuasarEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + this.Duckets + "', `credits` = '" + this.Credits + "', `vip_points` = '" + this.Diamonds + "', `home_room` = '" + this.HomeRoom + "', `gotw_points` = '" + this.GOTWPoints + "', `user_points` = '" + this.UserPoints + "', `time_muted` = '" + this.TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(this._friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + this._habboStats.RoomVisits + "', `onlineTime` = '" + (QuasarEnvironment.GetUnixTimestamp() - this.SessionStart + this._habboStats.OnlineTime) + "', `respect` = '" + this._habboStats.Respect + "', `respectGiven` = '" + this._habboStats.RespectGiven + "', `giftsGiven` = '" + this._habboStats.GiftsGiven + "', `giftsReceived` = '" + this._habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + this._habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + this._habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + this._habboStats.AchievementPoints + "', `quest_id` = '" + this._habboStats.QuestID + "', `quest_progress` = '" + this._habboStats.QuestProgress + "', `groupid` = '" + this._habboStats.FavouriteGroupId + "',`forum_posts` = '" + this._habboStats.ForumPosts + "' WHERE `id` = '" + this.Id + "' LIMIT 1;");

                    if (GetPermissions().HasRight("mod_tickets"))
                        dbClient.RunQuery("UPDATE `moderation_tickets` SET `status` = 'open', `moderator_id` = '0' WHERE `status` ='picked' AND `moderator_id` = '" + Id + "'");
                }
            }

            this.Dispose();

            this._client = null;

        }

        public void Dispose()
        {
            if (this.InventoryComponent != null)
                this.InventoryComponent.SetIdleState();

            if (this.UsersRooms != null)
                UsersRooms.Clear();

            if (this.InRoom && this.CurrentRoom != null)
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(this._client, false, false);

            if (Messenger != null)
            {
                this.Messenger.AppearOffline = true;
                this.Messenger.Destroy();
            }

            if (this._fx != null)
                this._fx.Dispose();

            if (this._clothing != null)
                this._clothing.Dispose();

            if (this._permissions != null)
                this._permissions.Dispose();
        }

        public void CheckCreditsTimer()
        {
            try
            {
                this._creditsTickUpdate--;

                if (this._creditsTickUpdate <= 0)
                {
                    int CreditUpdate = QuasarStaticGameSettings.UserCreditsUpdateAmount;
                    int DucketUpdate = QuasarStaticGameSettings.UserPixelsUpdateAmount;

                    SubscriptionData SubData = null;
                    if (QuasarEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(this._vipRank, out SubData))
                    {
                        CreditUpdate += SubData.Credits;
                        DucketUpdate += SubData.Duckets;
                    }

                    this._credits += CreditUpdate;
                    this._duckets += DucketUpdate;

                    this._client.SendMessage(new CreditBalanceComposer(this._credits));
                    this._client.SendMessage(new HabboActivityPointNotificationComposer(this._duckets, DucketUpdate));

                    this.CreditsUpdateTick = QuasarStaticGameSettings.UserCreditsUpdateTimer;
                }
            }
            catch { }
        }

        public GameClient GetClient()
        {
            if (this._client != null)
                return this._client;

            return QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
        }

        public HabboMessenger GetMessenger()
        {
            return Messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return BadgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return InventoryComponent;
        }

        public SearchesComponent GetNavigatorSearches()
        {
            return this._navigatorSearches;
        }

        public EffectsComponent Effects()
        {
            return this._fx;
        }

        public ClothingComponent GetClothing()
        {
            return this._clothing;
        }

        public int GetQuestProgress(int p)
        {
            int progress = 0;
            quests.TryGetValue(p, out progress);
            return progress;
        }

        public UserAchievement GetAchievementData(string p)
        {
            UserAchievement achievement = null;
            Achievements.TryGetValue(p, out achievement);
            return achievement;
        }

        public void ChangeName(string Username)
        {
            this.LastNameChange = QuasarEnvironment.GetUnixTimestamp();
            this.Username = Username;

            this.SaveKey("username", Username);
            this.SaveKey("last_change", this.LastNameChange.ToString());
        }

        public void SaveKey(string Key, string Value)
        {
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET " + Key + " = @value WHERE `id` = '" + this.Id + "' LIMIT 1;");
                dbClient.AddParameter("value", Value);
                dbClient.RunQuery();
            }
        }

        public void PrepareRoom(int Id, string Password)
        {
            if (this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return;

            if (this.GetClient().GetHabbo().InRoom)
            {
                Room OldRoom = null;
                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(this.GetClient().GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(this.GetClient(), false, false);
            }

            if (this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID != Id)
            {
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            Room Room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            if (Room == null)
            {
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            if (Room.isCrashed)
            {
                this.GetClient().SendNotification("La sala no está disponible en estos momentos, ponte en contacto con un administrador.");
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            this.GetClient().GetHabbo().CurrentRoomId = Room.RoomId;

            if (Room.GetRoomUserManager().userCount >= Room.UsersMax && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_full") && this.GetClient().GetHabbo().Id != Room.OwnerId)
            {
                this.GetClient().SendMessage(new CantConnectComposer(1));
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            if (!this.GetClient().GetHabbo().GetPermissions().HasRight("room_ban_override") && Room.UserIsBanned(this.GetClient().GetHabbo().Id))
            {
                if (Room.HasBanExpired(this.GetClient().GetHabbo().Id))
                    Room.RemoveBan(this.GetClient().GetHabbo().Id);
                else
                {
                    this.GetClient().GetHabbo().RoomAuthOk = false;
                    this.GetClient().SendMessage(new CantConnectComposer(4));
                    this.GetClient().SendMessage(new CloseConnectionComposer());
                    return;
                }
            }

            this.GetClient().SendMessage(new OpenConnectionComposer());
            if (!Room.CheckRights(this.GetClient(), true, true) && !this.GetClient().GetHabbo().IsTeleporting && !this.GetClient().GetHabbo().IsHopping)
            {
                if (Room.Access == RoomAccess.DOORBELL && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (Room.UserCount > 0)
                    {
                        this.GetClient().SendMessage(new DoorbellComposer(""));
                        Room.SendMessage(new DoorbellComposer(this.GetClient().GetHabbo().Username), true);
                        return;
                    }
                    else
                    {
                        this.GetClient().SendMessage(new FlatAccessDeniedComposer(""));
                        //Room.SendMessage(new RoomInfoUpdatedComposer(Room.RoomId));
                        this.GetClient().SendMessage(new CloseConnectionComposer());
                        return;
                    }
                }
                else if (Room.Access == RoomAccess.PASSWORD && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (Password.ToLower() != Room.Password.ToLower() || String.IsNullOrWhiteSpace(Password))
                    {
                        this.GetClient().SendMessage(new GenericErrorComposer(-100002));
                        this.GetClient().SendMessage(new CloseConnectionComposer());
                        return;
                    }
                }
            }

            if (!EnterRoom(Room))
                this.GetClient().SendMessage(new CloseConnectionComposer());

        }

        public bool EnterRoom(Room Room)
        {
            if (Room == null)
                this.GetClient().SendMessage(new CloseConnectionComposer());

            this.GetClient().SendMessage(new RoomReadyComposer(Room.RoomId, Room.ModelName));
            if (Room.Wallpaper != "0.0")
                this.GetClient().SendMessage(new RoomPropertyComposer("wallpaper", Room.Wallpaper));
            if (Room.Floor != "0.0")
                this.GetClient().SendMessage(new RoomPropertyComposer("floor", Room.Floor));

            this.GetClient().SendMessage(new RoomPropertyComposer("landscape", Room.Landscape));
            this.GetClient().SendMessage(new RoomRatingComposer(Room.Score, !(this.GetClient().GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.OwnerId == this.GetClient().GetHabbo().Id)));

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_roomvisits (user_id,room_id,entry_timestamp,exit_timestamp,hour,minute) VALUES ('" + this.GetClient().GetHabbo().Id + "','" + this.GetClient().GetHabbo().CurrentRoomId + "','" + QuasarEnvironment.GetUnixTimestamp() + "','0','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "');");// +
            }


            if (Room.OwnerId != this.Id)
            {
                this.GetClient().GetHabbo().GetStats().RoomVisits += 1;
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(this.GetClient(), "ACH_RoomEntry", 1);
                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(this.GetClient(), "ACH_ExploreRoom", 1);
           }

            return true;
        }
    }
    enum typeOfHelper
    {
        None,
        Guide,
        Alpha,
        Guardian
    }
}