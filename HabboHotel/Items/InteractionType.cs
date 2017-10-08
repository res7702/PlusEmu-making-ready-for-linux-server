using System;
using Quasar;
using Quasar.Core;
namespace Quasar.HabboHotel.Items
{
    public enum InteractionType
    {
        MAGICEGG,
        PREMIUM,
        HOLE,
        NONE,
        GATE,
        POSTIT,
        MOODLIGHT,
        TROPHY,
        wired_score_board,
        EffectAddScore,
        BED,
        SCOREBOARD,
        VENDING_MACHINE,
        ALERT,
        ONE_WAY_GATE,
        LOVE_SHUFFLER,
        HABBO_WHEEL,
        DICE,
        DICE2,
        POKER,
        BOTTLE,
        HOPPER,
        TELEPORT,
        POOL,
        SILLAGUIA,
        club_1_month,
        club_3_month,
        club_6_month,
        ROLLER,
        FOOTBALL_GATE,
        TRAX,

        // Dieren
        pet0,  // Hond
        pet1,  // Kat
        pet2,  // Krokodil
        pet3,  // Terrier
        pet4,  // Beren
        pet5,  // Varkens
        pet6,  // Leeuwen
        pet7,  // Neushoorns
        pet8,  // Spinnen
        pet9,  // Schildpadden
        pet10, // Kuikens
        pet11, // Kikkers
        pet12, // Draken
        pet13, // Monsters
        pet14, // Apen
        pet15, // Paarden
        pet16, // Monsterplant
        pet17, // Konijnen
        pet18, // Evil Konijnen
        pet19, // Depressieve Konijnen
        pet20, // Liefdes Konijnen
        pet21, // Witte Duiven
        pet22, // Zwarte Duiven
        pet23, // Bezeten Apen
        pet24, // Baby Beertjes
        pet25, // Baby Terriers
        pet26, // Kabouters
        pet27, // Baby's
        pet28, // Baby Kittens 
        pet29, // Baby Puppy's
        pet30, // Baby Varkens
        pet31, // Oempa Loempa's
        pet32, // Stenen
        pet33, // Pterodactylussen
        pet34, // Velociraptors
        pet35, // Wolven
        pet36, // Monster Konijnen
        pet37, // Pickachu
        pet38, // Pinguins
        pet39, // Mario
        pet40, // Olifanten
        pet41, // Alien Konijnen
        pet42, // Gouden Konijnen
        pet43, // Roze Mewtwo
        pet44, // Entei
        pet45, // Blauwe Mewtwo
        pet46, // Cavia
        pet47, // Uil
        pet48, // Goude Mewtwo
        pet49, // Eend
        pet50, // Baby Bruin
        pet51, // Baby Wit
        pet52, // Dino
        pet53, // Yoshi
        pet54, // Koe
        pet55, // reserved
        pet56,
        pet57,
        pet58,
        pet59,
        pet60,
        pet61,
        pet62,
        pet63,
        pet64,
        pet65,
        pet66,
        pet67,
        pet68,
        pet69,
        pet70,

        ICE_SKATES,
        NORMAL_SKATES,
        lowpool,
        haloweenpool,
        FOOTBALL,
        FOOTBALL_GOAL_GREEN,
        FOOTBALL_GOAL_YELLOW,
        FOOTBALL_GOAL_BLUE,
        FOOTBALL_GOAL_RED,
        footballcountergreen,
        footballcounteryellow,
        footballcounterblue,
        footballcounterred,
        banzaigateblue,
        banzaigatered,
        banzaigateyellow,
        banzaigategreen,
        banzaifloor,
        banzaiscoreblue,
        banzaiscorered,
        banzaiscoreyellow,
        banzaiscoregreen,
        banzaicounter,
        banzaitele,
        banzaipuck,
        banzaipyramid,
        freezetimer,
        freezeexit,
        freezeredcounter,
        freezebluecounter,
        freezeyellowcounter,
        freezegreencounter,
        FREEZE_YELLOW_GATE,
        FREEZE_RED_GATE,
        FREEZE_GREEN_GATE,
        FREEZE_BLUE_GATE,
        FREEZE_TILE_BLOCK,
        FREEZE_TILE,
        JUKEBOX,
        MUSIC_DISC,
        PUZZLE_BOX,
        TONER,
        CRAFTING,

        PRESSURE_PAD,

        WF_FLOOR_SWITCH_1,
        WF_FLOOR_SWITCH_2,

        GIFT,
        BACKGROUND,
        MANNEQUIN,
        GATE_VIP,
        GUILD_ITEM,
        GUILD_GATE,
        GUILD_FORUM,

        TENT,
        TENT_SMALL,
        BADGE_DISPLAY,
        STACKTOOL,
        TELEVISION,

        WIRED_EFFECT,
        WIRED_TRIGGER,
        WIRED_CONDITION,

        WALLPAPER,
        FLOOR,
        LANDSCAPE,

        BADGE,
        CRACKABLE_EGG,
        EFFECT,
        DEAL,

        HORSE_SADDLE_1,
        HORSE_SADDLE_2,
        HORSE_HAIRSTYLE,
        HORSE_BODY_DYE,
        HORSE_HAIR_DYE,

        GNOME_BOX,
        BOT,
        PURCHASABLE_CLOTHING,
        PET_BREEDING_BOX,
        ARROW,
        LOVELOCK,
        MONSTERPLANT_SEED,
        CANNON,
        COUNTER,
        CAMERA_PICTURE,
        PINATA,
        INFO_TERMINAL,
        FX_PROVIDER,
        PINATATRIGGERED
    }


    public class InteractionTypes
    {
        public static InteractionType GetTypeFromString(string pType)
        {
            switch (pType.ToLower())
            {
                case "default":
                    return InteractionType.NONE;
                case "hole":
                    return InteractionType.HOLE;
                case "gate":
                    return InteractionType.GATE;
                case "postit":
                    return InteractionType.POSTIT;
                case "dimmer":
                    return InteractionType.MOODLIGHT;
                case "wired_score_board":
                    return InteractionType.wired_score_board;
                case "trophy":
                    return InteractionType.TROPHY;
                case "bed":
                    return InteractionType.BED;
                case "scoreboard":
                    return InteractionType.SCOREBOARD;
                case "vendingmachine":
                    return InteractionType.VENDING_MACHINE;
                case "alert":
                    return InteractionType.ALERT;
                case "onewaygate":
                    return InteractionType.ONE_WAY_GATE;
                case "loveshuffler":
                    return InteractionType.LOVE_SHUFFLER;
                case "habbowheel":
                    return InteractionType.HABBO_WHEEL;
                case "dice":
                    return InteractionType.DICE;
                case "dice2":
                    return InteractionType.DICE2;
                case "pokerset":
                    return InteractionType.POKER;
                case "hopper":
                    return InteractionType.HOPPER;
                case "bottle":
                    return InteractionType.BOTTLE;
                case "teleport":
                    return InteractionType.TELEPORT;
                case "pool":
                    return InteractionType.POOL;
                case "sillaguia":
                    return InteractionType.SILLAGUIA;
                case "roller":
                    return InteractionType.ROLLER;
                case "fbgate":
                    return InteractionType.FOOTBALL_GATE;
                case "pet0": // Hond
                    return InteractionType.pet0;
                case "pet1": // Kat
                    return InteractionType.pet1;
                case "pet2": // Krokodil
                    return InteractionType.pet2;
                case "pet3": // Terrier
                    return InteractionType.pet3;
                case "pet4": // Beren
                    return InteractionType.pet4;
                case "pet5": // Varkens
                    return InteractionType.pet5;
                case "pet6": // Leeuwen
                    return InteractionType.pet6;
                case "pet7": // Neushoorns
                    return InteractionType.pet7;
                case "pet8": // Spinnen
                    return InteractionType.pet8;
                case "pet9": // Schildpadden
                    return InteractionType.pet9;
                case "pet10": // Kuikens
                    return InteractionType.pet10;
                case "pet11": // Kikkers
                    return InteractionType.pet11;
                case "pet12": // Draken
                    return InteractionType.pet12;
                case "pet13": // Monsters
                    return InteractionType.pet13;
                case "pet14": // Apen
                    return InteractionType.pet14;
                case "pet15": // Paarden
                    return InteractionType.pet15;
                case "pet16": // Monsterplanten
                    return InteractionType.pet16;
                case "pet17": // Konijnen
                    return InteractionType.pet17;
                case "pet18": // Evil Konijnen
                    return InteractionType.pet18;
                case "pet19": // Depressieve Konijnen
                    return InteractionType.pet19;
                case "pet20": // Liefdes Konijnen
                    return InteractionType.pet20;
                case "pet21": // Witte Duiven
                    return InteractionType.pet21;
                case "pet22": // Zwarte Duiven
                    return InteractionType.pet22;
                case "pet23": // Rode Apen
                    return InteractionType.pet23;
                case "pet24": // Baby Beertjes
                    return InteractionType.pet24;
                case "pet25": // Baby Terriers
                    return InteractionType.pet25;
                case "pet26": // Kabouters
                    return InteractionType.pet26;
                case "pet27": // Baby's
                    return InteractionType.pet27;
                case "pet28": // Baby Kittens
                    return InteractionType.pet28;
                case "pet29": // Baby Puppy's
                    return InteractionType.pet29;
                case "pet30": // Baby Varkens
                    return InteractionType.pet30;
                case "pet31": // Oempa Loempa's
                    return InteractionType.pet31;
                case "pet32": // Stenen
                    return InteractionType.pet32;
                case "pet33": // Pterodactylussen
                    return InteractionType.pet33;
                case "pet34": // Velociraptors
                    return InteractionType.pet34;
                case "pet35": // Wolven
                    return InteractionType.pet35;
                case "pet36": // Monster Konijnen
                    return InteractionType.pet36;
                case "pet37": // Pickachu
                    return InteractionType.pet37;
                case "pet38": // Pinguins
                    return InteractionType.pet38;
                case "pet39": // Mario
                    return InteractionType.pet39;
                case "pet40": // Olifanten
                    return InteractionType.pet40;
                case "pet41": // Alien Konijnen
                    return InteractionType.pet41;
                case "pet42": // Gouden Konijnen
                    return InteractionType.pet42;
                case "pet43": // Roze Mewtwo
                    return InteractionType.pet43;
                case "pet44": // Entei
                    return InteractionType.pet44;
                case "pet45": // Blauwe Mewtwo
                    return InteractionType.pet45;
                case "pet46": // Cavia
                    return InteractionType.pet46;
                case "pet47": // Uil
                    return InteractionType.pet47;
                case "pet48": // Goude Mewtwo
                    return InteractionType.pet48;
                case "pet49": // Eend
                    return InteractionType.pet49;
                case "pet50": // Baby Bruin
                    return InteractionType.pet50;
                case "pet51": // Baby Wit
                    return InteractionType.pet51;
                case "pet52": // Dino Ei
                    return InteractionType.pet52;
                case "pet53": // Yoshi
                    return InteractionType.pet53;
                case "pet54": // Koe
                    return InteractionType.pet54;
                case "pet55": // reserved
                    return InteractionType.pet55;
                case "pet56": // reserved
                    return InteractionType.pet56;
                case "pet57": // reserved
                    return InteractionType.pet57;
                case "pet58": // reserved
                    return InteractionType.pet58;
                case "pet59": // reserved
                    return InteractionType.pet59;
                case "pet60": // reserved
                    return InteractionType.pet60;
                case "pet61": // reserved
                    return InteractionType.pet61;
                case "pet62": // reserved
                    return InteractionType.pet62;
                case "pet63": // reserved
                    return InteractionType.pet63;
                case "pet64": // reserved
                    return InteractionType.pet64;
                case "pet65": // reserved
                    return InteractionType.pet65;
                case "pet66": // reserved
                    return InteractionType.pet66;
                case "pet67": // reserved
                    return InteractionType.pet67;
                case "pet68": // reserved
                    return InteractionType.pet68;
                case "pet69": // reserved
                    return InteractionType.pet69;
                case "pet70": // reserved
                    return InteractionType.pet70;
                case "iceskates":
                    return InteractionType.ICE_SKATES;
                case "rollerskate":
                    return InteractionType.NORMAL_SKATES;
                case "lowpool":
                    return InteractionType.lowpool;
                case "haloweenpool":
                    return InteractionType.haloweenpool;
                case "ball":
                    return InteractionType.FOOTBALL;
                case "premium":
                    return InteractionType.PREMIUM;
                case "club_1_month":
                    return InteractionType.club_1_month;
                case "club_3_month":
                    return InteractionType.club_3_month;
                case "club_6_month":
                    return InteractionType.club_6_month;

                case "green_goal":
                    return InteractionType.FOOTBALL_GOAL_GREEN;
                case "yellow_goal":
                    return InteractionType.FOOTBALL_GOAL_YELLOW;
                case "red_goal":
                    return InteractionType.FOOTBALL_GOAL_RED;
                case "blue_goal":
                    return InteractionType.FOOTBALL_GOAL_BLUE;

                case "green_score":
                    return InteractionType.footballcountergreen;
                case "yellow_score":
                    return InteractionType.footballcounteryellow;
                case "blue_score":
                    return InteractionType.footballcounterblue;
                case "red_score":
                    return InteractionType.footballcounterred;

                case "bb_blue_gate":
                    return InteractionType.banzaigateblue;
                case "bb_red_gate":
                    return InteractionType.banzaigatered;
                case "bb_yellow_gate":
                    return InteractionType.banzaigateyellow;
                case "bb_green_gate":
                    return InteractionType.banzaigategreen;
                case "bb_patch":
                    return InteractionType.banzaifloor;

                case "bb_blue_score":
                    return InteractionType.banzaiscoreblue;
                case "bb_red_score":
                    return InteractionType.banzaiscorered;
                case "bb_yellow_score":
                    return InteractionType.banzaiscoreyellow;
                case "bb_green_score":
                    return InteractionType.banzaiscoregreen;

                case "banzaicounter":
                    return InteractionType.banzaicounter;
                case "bb_teleport":
                    return InteractionType.banzaitele;
                case "banzaipuck":
                    return InteractionType.banzaipuck;
                case "bb_pyramid":
                    return InteractionType.banzaipyramid;

                case "freezetimer":
                    return InteractionType.freezetimer;
                case "freezeexit":
                    return InteractionType.freezeexit;
                case "freezeredcounter":
                    return InteractionType.freezeredcounter;
                case "freezebluecounter":
                    return InteractionType.freezebluecounter;
                case "freezeyellowcounter":
                    return InteractionType.freezeyellowcounter;
                case "freezegreencounter":
                    return InteractionType.freezegreencounter;
                case "freezeyellowgate":
                    return InteractionType.FREEZE_YELLOW_GATE;
                case "freezeredgate":
                    return InteractionType.FREEZE_RED_GATE;
                case "freezegreengate":
                    return InteractionType.FREEZE_GREEN_GATE;
                case "freezebluegate":
                    return InteractionType.FREEZE_BLUE_GATE;
                case "freezetileblock":
                    return InteractionType.FREEZE_TILE_BLOCK;
                case "freezetile":
                    return InteractionType.FREEZE_TILE;

                case "jukebox":
                    return InteractionType.JUKEBOX;
                case "musicdisc":
                    return InteractionType.MUSIC_DISC;

                case "pressure_pad":
                    return InteractionType.PRESSURE_PAD;
                case "wf_floor_switch1":
                    return InteractionType.WF_FLOOR_SWITCH_1;
                case "wf_floor_switch2":
                    return InteractionType.WF_FLOOR_SWITCH_2;
                case "puzzlebox":
                    return InteractionType.PUZZLE_BOX;
                case "water":
                    return InteractionType.POOL;
                case "gift":
                    return InteractionType.GIFT;
                case "background":
                    return InteractionType.BACKGROUND;
                case "mannequin":
                    return InteractionType.MANNEQUIN;
                case "vip_gate":
                    return InteractionType.GATE_VIP;
                case "roombg":
                    return InteractionType.TONER;
                case "gld_item":
                    return InteractionType.GUILD_ITEM;
                case "gld_gate":
                    return InteractionType.GUILD_GATE;
                case "guild_forum":
                    return InteractionType.GUILD_FORUM;
                case "tent":
                    return InteractionType.TENT;
                case "tent_small":
                    return InteractionType.TENT_SMALL;

                case "badge_display":
                    return InteractionType.BADGE_DISPLAY;
                case "stacktool":
                    return InteractionType.STACKTOOL;
                case "television":
                    return InteractionType.TELEVISION;


                case "wired_effect":
                    return InteractionType.WIRED_EFFECT;
                case "wired_trigger":
                    return InteractionType.WIRED_TRIGGER;
                case "wired_condition":
                    return InteractionType.WIRED_CONDITION;

                case "floor":
                    return InteractionType.FLOOR;
                case "wallpaper":
                    return InteractionType.WALLPAPER;
                case "landscape":
                    return InteractionType.LANDSCAPE;

                case "badge":
                    return InteractionType.BADGE;

                case "crackable_egg":
                    return InteractionType.CRACKABLE_EGG;
                case "effect":
                    return InteractionType.EFFECT;
                case "deal":
                    return InteractionType.DEAL;

                case "horse_saddle_1":
                    return InteractionType.HORSE_SADDLE_1;
                case "horse_saddle_2":
                    return InteractionType.HORSE_SADDLE_2;
                case "horse_hairstyle":
                    return InteractionType.HORSE_HAIRSTYLE;
                case "horse_body_dye":
                    return InteractionType.HORSE_BODY_DYE;
                case "horse_hair_dye":
                    return InteractionType.HORSE_HAIR_DYE;

                case "gnome_box":
                    return InteractionType.GNOME_BOX;
                case "bot":
                    return InteractionType.BOT;
                case "purchasable_clothing":
                    return InteractionType.PURCHASABLE_CLOTHING;
                case "pet_breeding_box":
                    return InteractionType.PET_BREEDING_BOX;
                case "arrow":
                    return InteractionType.ARROW;
                case "lovelock":
                    return InteractionType.LOVELOCK;
                case "cannon":
                    return InteractionType.CANNON;
                case "counter":
                    return InteractionType.COUNTER;
                case "camera_picture":
                    return InteractionType.CAMERA_PICTURE;
                case "provider":
                    return InteractionType.FX_PROVIDER;
                case "pinata":
                    return InteractionType.PINATA;
                case "info_terminal":
                    return InteractionType.INFO_TERMINAL;
                case "pinatayihadista":
                    return InteractionType.PINATATRIGGERED;
                case "crafting":
                    return InteractionType.CRAFTING;
                case "magicegg":
                    return InteractionType.MAGICEGG;


                default:
                    {
                        //Logging.WriteLine("Unknown interaction type in parse code: " + pType, ConsoleColor.Yellow);
                        return InteractionType.NONE;
                    }
            }
        }
    }
}