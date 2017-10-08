
using log4net;

using Quasar.Communication.Packets;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Moderation;
using Quasar.HabboHotel.Support;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Items.Televisions;
using Quasar.HabboHotel.Navigator;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Achievements;
using Quasar.HabboHotel.LandingView;
using Quasar.HabboHotel.Global;

using Quasar.HabboHotel.Games;

using Quasar.HabboHotel.Rooms.Chat;
using Quasar.HabboHotel.Talents;
using Quasar.HabboHotel.Bots;
using Quasar.HabboHotel.Cache;
using Quasar.HabboHotel.Rewards;
using Quasar.HabboHotel.Badges;
using Quasar.HabboHotel.Permissions;
using Quasar.HabboHotel.Subscriptions;
using System.Threading;
using System.Threading.Tasks;
using Quasar.HabboHotel.Camera;
using Quasar.HabboHotel.Catalog.FurniMatic;
using Quasar.Communication.Packets.Incoming.LandingView;
using Quasar.HabboHotel.Helpers;
using Quasar.HabboHotel.Groups.Forums;
using Quasar.HabboHotel.Rooms.TraxMachine;
using System;
using Quasar.HabboHotel.Calendar;
using Quasar.HabboHotel.Items.Crafting;

namespace Quasar.HabboHotel
{
    public class Game
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Game");

        public GroupForumManager GetGroupForumManager()
        {
            return forummanager;
        }

        private GroupForumManager forummanager;
        private readonly PacketManager _packetManager;
        private readonly GameClientManager _clientManager;
        private readonly ModerationManager _modManager;
        private readonly ModerationTool _moderationTool;//TODO: Initialize from the moderation manager.
        private readonly ItemDataManager _itemDataManager;
        private readonly CatalogManager _catalogManager;
        private readonly TelevisionManager _televisionManager;//TODO: Initialize from the item manager.
        private readonly NavigatorManager _navigatorManager;
        private readonly RoomManager _roomManager;
        private readonly ChatManager _chatManager;
        private readonly GroupManager _groupManager;
        private readonly QuestManager _questManager;
        private readonly AchievementManager _achievementManager;
        private readonly TalentTrackManager _talentTrackManager;
        private readonly LandingViewManager _landingViewManager;//TODO: Rename class
        private readonly GameDataManager _gameDataManager;
        private readonly CraftingManager _craftingManager;
        private readonly ServerStatusUpdater _globalUpdater;
        //private readonly LanguageLocale _languageLocale;
        private readonly AntiMutant _antiMutant;
        private readonly BotManager _botManager;
        private readonly CacheManager _cacheManager;
        private readonly RewardManager _rewardManager;
        private readonly BadgeManager _badgeManager;
        private readonly PermissionManager _permissionManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly TargetedOffersManager _targetedoffersManager;
        private readonly CrackableManager _crackableManager;
        private readonly FurniMaticRewardsManager _furniMaticRewardsManager;
        private readonly TalentManager _talentManager;
        private CatalogFrontPage _catalogFrontPageManager;
        private readonly CalendarManager _calendarManager;

        private bool _cycleEnded;
        private bool _cycleActive;
        private Task _gameCycle;
        private int _cycleSleepTime = 25;

        public Game()
        {
            this._packetManager = new PacketManager();
            this._clientManager = new GameClientManager();
            this._modManager = new ModerationManager();
            this._moderationTool = new ModerationTool();

            this._itemDataManager = new ItemDataManager();
            this._itemDataManager.Init();

            this._catalogFrontPageManager = new CatalogFrontPage();
            this._catalogManager = new CatalogManager();
            this._catalogManager.Init(this._itemDataManager);

            this._televisionManager = new TelevisionManager();
            this._crackableManager = new CrackableManager();
            this._crackableManager.Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
            this._furniMaticRewardsManager = new FurniMaticRewardsManager();
            this._furniMaticRewardsManager.Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());

            this._craftingManager = new CraftingManager();
            this._craftingManager.Init();

            this._navigatorManager = new NavigatorManager();
            this._roomManager = new RoomManager();
            this._chatManager = new ChatManager();
            this._groupManager = new GroupManager();
            this._questManager = new QuestManager();
            this._achievementManager = new AchievementManager();
            this._talentManager = new TalentManager();
            this._talentManager.Initialize();
            this._talentTrackManager = new TalentTrackManager();

            this._landingViewManager = new LandingViewManager();
            this._gameDataManager = new GameDataManager();

            this._globalUpdater = new ServerStatusUpdater();
            this._globalUpdater.Init();

            //this._languageLocale = new LanguageLocale();
            this._antiMutant = new AntiMutant();
            this._botManager = new BotManager();

            this._cacheManager = new CacheManager();
            this._rewardManager = new RewardManager();

            this._badgeManager = new BadgeManager();
            this._badgeManager.Init();

            this.forummanager = new GroupForumManager();

            TraxSoundManager.Init(); // Added
            GetHallOfFame.getInstance().Load();

            this._permissionManager = new PermissionManager();
            this._permissionManager.Init();

            this._subscriptionManager = new SubscriptionManager();
            this._subscriptionManager.Init();

            HelperToolsManager.Init();

            this._targetedoffersManager = new TargetedOffersManager();
            this._targetedoffersManager.Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());

            this._calendarManager = new CalendarManager();
            this._calendarManager.Init();
        }

        public void StartGameLoop()
        {
            this._gameCycle = new Task(GameCycle);
            this._gameCycle.Start();

            this._cycleActive = true;
        }

        private void GameCycle()
        {
            while (this._cycleActive)
            {
                this._cycleEnded = false;

                QuasarEnvironment.GetGame().GetRoomManager().OnCycle();
                QuasarEnvironment.GetGame().GetClientManager().OnCycle();
                //AlphaManager.getInstance().onCycle();
                this._cycleEnded = true;
                Thread.Sleep(this._cycleSleepTime);
            }
        }

        public void StopGameLoop()
        {
            this._cycleActive = false;

            while (!this._cycleEnded)
            {
                Thread.Sleep(this._cycleSleepTime);
            }
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public GameClientManager GetClientManager()
        {
            return _clientManager;
        }

        public CatalogManager GetCatalog()
        {
            return _catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return _navigatorManager;
        }

        public ItemDataManager GetItemManager()
        {
            return _itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return _roomManager;
        }

        internal TargetedOffersManager GetTargetedOffersManager()
        {
            return _targetedoffersManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return _achievementManager;
        }

        public CatalogFrontPage getCatalogFrontPage()
        {
            return _catalogFrontPageManager;
        }

        public TalentTrackManager GetTalentTrackManager()
        {
            return _talentTrackManager;
        }

        public ModerationTool GetModerationTool()
        {
            return _moderationTool;
        }

        public TalentManager GetTalentManager()
        {
            return _talentManager;
        }

        public ModerationManager GetModerationManager()
        {
            return this._modManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return this._permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return this._subscriptionManager;
        }

        public QuestManager GetQuestManager()
        {
            return this._questManager;
        }

        public GroupManager GetGroupManager()
        {
            return _groupManager;
        }

        public LandingViewManager GetLandingManager()
        {
            return _landingViewManager;
        }
        public TelevisionManager GetTelevisionManager()
        {
            return _televisionManager;
        }

        public ChatManager GetChatManager()
        {
            return this._chatManager;
        }

        internal CrackableManager GetPinataManager()
        {
            return this._crackableManager;
        }

        public CraftingManager GetCraftingManager()
        {
            return this._craftingManager;
        }

        public FurniMaticRewardsManager GetFurniMaticRewardsMnager()
        {
            return this._furniMaticRewardsManager;
        }

        public GameDataManager GetGameDataManager()
        {
            return this._gameDataManager;
        }

        //public LanguageLocale GetLanguageLocale()
        //{
        //    return this._languageLocale;
        //}

        public AntiMutant GetAntiMutant()
        {
            return this._antiMutant;
        }

        public BotManager GetBotManager()
        {
            return this._botManager;
        }

        public CacheManager GetCacheManager()
        {
            return this._cacheManager;
        }

        public RewardManager GetRewardManager()
        {
            return this._rewardManager;
        }

        public CalendarManager GetCalendarManager()
        {
            return _calendarManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return this._badgeManager;
        }
    }
}