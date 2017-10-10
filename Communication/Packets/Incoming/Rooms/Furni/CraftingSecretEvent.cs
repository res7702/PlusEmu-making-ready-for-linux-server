using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Communication.Packets.Outgoing.Rooms.Furni;
using Quasar.HabboHotel.Items.Crafting;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class CraftSecretEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int craftingTable = Packet.PopInt();
            //int itemCount = Packet.PopInt();

            //int[] myItems = new int[itemCount];
            //for (int i = 0; i < itemCount; itemCount++)
            //{
            //    int ItemID = Packet.PopInt();
            //    Item InventoryItem = Session.GetHabbo().GetInventoryComponent().GetItem(ItemID);
            //    if (InventoryItem == null)
            //        continue;

            //    myItems[i] = InventoryItem.BaseItem;
            //}

            List<Item> items = new List<Item>();

            var count = Packet.PopInt();
            for (var i = 1; i <= count; i++)
            {
                var id = Packet.PopInt();

                var item = Session.GetHabbo().GetInventoryComponent().GetItem(id);
                if (item == null || items.Contains(item))
                    return;

                items.Add(item);
            }

            CraftingRecipe recipe = null;
            foreach (var Receta in QuasarEnvironment.GetGame().GetCraftingManager().CraftingRecipes)
            {
                bool found = false;

                foreach (var item in Receta.Value.ItemsNeeded)
                {
                    if (item.Value != items.Count(item2 => item2.GetBaseItem().ItemName == item.Key))
                    {
                        found = false;
                        break;
                    }

                    found = true;
                }

                if (found == false)
                    continue;

                recipe = Receta.Value;
                //Console.WriteLine(recipe.type);
                break;
            }
            if (recipe == null) return;
            ItemData resultItem = QuasarEnvironment.GetGame().GetItemManager().GetItemByName(recipe.Result);
            if (resultItem == null) return;
            bool success = true;
            foreach (var need in recipe.ItemsNeeded)
            {
                for (var i = 1; i <= need.Value; i++)
                {
                    ItemData item = QuasarEnvironment.GetGame().GetItemManager().GetItemByName(need.Key);
                    if (item == null)
                    {
                        success = false;
                        continue;
                    }

                    var inv = Session.GetHabbo().GetInventoryComponent().GetFirstItemByBaseId(item.Id);
                    if (inv == null)
                    {
                        success = false;
                        continue;
                    }

                    using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor()) dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + inv.Id + "' AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(inv.Id);
                }
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            if (success)
            {
                Session.GetHabbo().GetInventoryComponent().AddNewItem(0, resultItem.Id, "", 0, true, false, 0, 0);
                Session.SendMessage(new FurniListUpdateComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(true); // This did update it right away few min ago so try this :o
                Session.SendMessage(new CraftableProductsComposer());


                #region Achievements
                // #1 hween11_sofa
                if (resultItem.Id == 2683)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSofa", 1);
                }
                // #2 hween12_floor
                if (resultItem.Id == 3150)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFloorBones", 1);
                }
                // #3 hween12_lantern
                if (resultItem.Id == 3146)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLantern", 1);
                }
                // #4 hween14_bed
                if (resultItem.Id == 4608)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBedKleur", 1);
                }
                // #5 hween14_demon
                if (resultItem.Id == 4615)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenStier", 1);
                }
                // #6 hween_c15_shinycarpet
                if (resultItem.Id == 4772)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenShinyCarpet", 1);
                }
                // #7 st_hween14_mbox
                if (resultItem.Id == 4620)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBox", 1);
                }
                // #8 st_hween14_closet
                if (resultItem.Id == 4622)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenCloset", 1);
                }
                // #9 hween_r16_grandpiano2
                if (resultItem.Id == 9034)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPiano", 1);
                }
                // #10 hween_r16_chandelier2
                if (resultItem.Id == 9031)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChandelier", 1);
                }
                // #11 hween_c16_wall2
                if (resultItem.Id == 9028)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenWall", 1);
                }
                // #12 hween_c16_vase2
                if (resultItem.Id == 9026)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenVase", 1);
                }
                // #13 hween_c16_vanity2
                if (resultItem.Id == 9024)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenVanity", 1);
                }
                // #14 hween_c16_roundtable2
                if (resultItem.Id == 9021)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenRoundTable", 1);
                }
                // #15 hween_c16_lamp2
                if (resultItem.Id == 9019)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLamp", 1);
                }
                // #16 hween_c16_ladder2
                if (resultItem.Id == 9017)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLadder", 1);
                }
                // #17 hween_c16_glasstable2
                if (resultItem.Id == 9015)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGlasstable", 1);
                }
                // #18 hween_c16_ghostvial
                if (resultItem.Id == 9013)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostVial", 1);
                }
                // #19 hween_c16_ghostorb
                if (resultItem.Id == 9012)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostOrb", 1);
                }
                // #20 hween_c16_ghostash
                if (resultItem.Id == 9010)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostAsh", 1);
                }
                // #21 hween_c16_floor2
                if (resultItem.Id == 9009)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFloor", 1);
                }
                // #22 hween_c16_fireplace2
                if (resultItem.Id == 9007)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFireplace", 1);
                }
                // #23 hween_c16_endtable2
                if (resultItem.Id == 9005)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEndtable", 1);
                }
                // #24 hween_c16_chair2
                if (resultItem.Id == 9002)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChair", 1);
                }
                // #25 hween_c16_cabinet2
                if (resultItem.Id == 9000)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenCabinet", 1);
                }
                // #26 hween_c16_bust2
                if (resultItem.Id == 8998)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBust", 1);
                }
                // #27 hween_c16_bkcase2
                if (resultItem.Id == 8996)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBkcase", 1);
                }
                // #28 hween_c16_bed2
                if (resultItem.Id == 8994)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBed", 1);
                }
                // #29 hween_c16_barchair2
                if (resultItem.Id == 8992)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBarchair", 1);
                }
                // #30 hween_c16_balcony2
                if (resultItem.Id == 8988)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBalcony", 1);
                }
                // #31 hween_c16_bar2
                if (resultItem.Id == 8990)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBar", 1);
                }
                // #32 hween10_zombie
                if (resultItem.Id == 2051)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenZombie", 1);
                }
                // #33 jungle_c16_bkcase2
                if (resultItem.Id == 9054)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBkcaseBrown", 1);
                }
                // #34 jungle_c16_bridgeend2
                if (resultItem.Id == 9057)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgeendBrown", 1);
                }
                // #35 jungle_c16_dvdr2
                if (resultItem.Id == 9062)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDvdrBrown", 1);
                }
                // #36 jungle_c16_gate2
                if (resultItem.Id == 9079)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleGate", 1);
                }
                // #37 jungle_c16_mat2
                if (resultItem.Id == 9083)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleMat", 1);
                }
                // #38 jungle_c16_pot2
                if (resultItem.Id == 9087)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePotPink", 1);
                }
                // #39 jungle_c16_stairs2
                if (resultItem.Id == 9097)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleStairsBrown", 1);
                }
                // #40 jungle_c16_roof2
                if (resultItem.Id == 9093)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleRoofPink", 1);
                }
                // #41 jungle_c16_swingsofa2
                if (resultItem.Id == 9100)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSwingsofaPink", 1);
                }
                // #42 jungle_c16_table2
                if (resultItem.Id == 9103)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTablePink", 1);
                }
                // #43 jungle_c16_treestage2
                if (resultItem.Id == 9109)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreestage", 1);
                }
                // #44 jungle_c16_wall2
                if (resultItem.Id == 9112)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWallBrown", 1);
                }
                // #45 jungle_c16_bkcase3
                if (resultItem.Id == 9055)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBkcaseGrey", 1);
                }
                // #46 jungle_c16_bridgeend3
                if (resultItem.Id == 9058)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgeendGrey", 1);
                }
                // #47 jungle_c16_bridgemid3
                if (resultItem.Id == 9513) // Add item
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgemidGrey", 1);
                }
                // #48 jungle_c16_dvdr3
                if (resultItem.Id == 9063)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDvdrGrey", 1);
                }
                // #49 jungle_c16_gate3 // Add image
                if (resultItem.Id == 9080)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChair", 1);
                }
                // #50 jungle_c16_mat3
                if (resultItem.Id == 9084)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleMatLighter", 1);
                }
                // #51 jungle_c16_pot3
                if (resultItem.Id == 9088)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePotBlue", 1);
                }
                // #52 jungle_c16_stairs3
                if (resultItem.Id == 9098)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleStairsGrey", 1);
                }
                // #53 jungle_c16_roof3
                if (resultItem.Id == 9094)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleRoofBlue", 1);
                }
                // #54 jungle_c16_swingsofa3
                if (resultItem.Id == 9101)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSwingsofaBlue", 1);
                }
                // #55 jungle_c16_table3
                if (resultItem.Id == 9104)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTableBlue", 1);
                }
                // #56 jungle_c16_treestage3
                if (resultItem.Id == 9110)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreestageRock", 1);
                }
                // #57 jungle_c16_wall3
                if (resultItem.Id == 9113)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWallGrey", 1);
                }
                // #58 anc_talltree
                if (resultItem.Id == 3054)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingTalltree", 1);
                }
                // #59 val13_water
                if (resultItem.Id == 3267)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingValentineWater", 1);
                }
                // #60 anc_comfy_tree
                if (resultItem.Id == 3057)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleComfyTree", 1);
                }
                // #61 stories_shakespeare_tree
                if (resultItem.Id == 4136)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleShakesphereTree", 1);
                }
                // #62 anc_waterfall
                if (resultItem.Id == 3055)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWaterfall", 1);
                }
                // #63 dino_c15_tree2
                if (resultItem.Id == 5695)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDinotree", 1);
                }
                // #64 lt_c15_bush // Added item
                if (resultItem.Id == 9514)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBush", 1);
                }
                // #65 lm_pond 
                if (resultItem.Id == 2490)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePond", 1);
                }
                // #66 tiki_c15_wall
                if (resultItem.Id == 6693)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWall", 1);
                }
                // #67 ny2013_cup
                if (resultItem.Id == 3248)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleCup", 1);
                }
                // #68 easter14_grasspatch
                if (resultItem.Id == 4242)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingEasterGrass", 1);
                }
                // #69 dino_c15_tree1
                if (resultItem.Id == 5694)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDinotreeSmall", 1);
                }
                // #70 val13_shrub_circ
                if (resultItem.Id == 3339)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleShrub", 1);
                }
                // #71 jetset_landhigh
                if (resultItem.Id == 3111)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJetsetLand", 1);
                }
                // #72 anc_sun
                if (resultItem.Id == 3049)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSun", 1);
                }
                // #73 hween08_sink
                if (resultItem.Id == 1363)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSink", 1);
                }
                // #74 hween08_bath
                if (resultItem.Id == 1367)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBath", 1);
                }
                // #75 dng_treasure
                if (resultItem.Id == 2426)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreasure", 1);
                }
                // #76 hween15_horseman5
                if (resultItem.Id == 4742)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenHorseman", 1);
                }
                // #77 hween_c15_purecrystal2
                if (resultItem.Id == 4766)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPureCrystal", 1);
                }
                // #78 hween_c15_purecrystal3
                if (resultItem.Id == 4767)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPureCrystalBig", 1);
                }
                // #79 hween_c15_evilcrystal2
                if (resultItem.Id == 4758)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEvilCrystal", 1);
                }
                // #80 hween_c15_evilcrystal3
                if (resultItem.Id == 4759)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEvilCrystaBig", 1);
                }
                // #81 qt_xm10_iceduck
                if (resultItem.Id == 2142)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingXmasIceDuck", 1);
                }
                // #82 deadduck
                if (resultItem.Id == 187)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDeadDuck", 1);
                }
                // #83 skullcandle
                if (resultItem.Id == 186)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSkullCandle", 1);
                }
                // #84 hween14_skelepieces // Add image
                if (resultItem.Id == 4579)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSkeletPieces", 1);
                }
                // #85 penguin_glow // Add Image
                if (resultItem.Id == 1412)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingPenguinGlow", 1);
                }
                // #86 hween13_bldtrail // Add image
                if (resultItem.Id == 3702)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBlood", 1);
                }
                // #87 LT_skull // Add image
                if (resultItem.Id == 1608)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingSkull", 1);
                }
                // #88 guitar_skull // Add image
                if (resultItem.Id == 5849)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingGuitarSkull", 1);
                }
                // #89 hween14_doll4
                if (resultItem.Id == 4605)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDoll", 1);
                }
                // #90 hween14_doll3
                if (resultItem.Id == 4604)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDollAfro", 1);
                }
                //# 91 hween14_mariachi
                if (resultItem.Id == 4580)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenMariachi", 1);
                }
                // #92 hween12_guillotine
                if (resultItem.Id == 3142)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGuillotine", 1);
                }
                // #93 gothic_c15_chandelier
                if (resultItem.Id == 6584)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChandelierDead", 1);
                }
                // #94 jungle_c16_bridgemid2
                if (resultItem.Id == 6584)
                {
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, " ACH_CraftingJungleBridgemidBrown1", 1);
                }
              #endregion
            }
            Session.SendMessage(new CraftingResultComposer(recipe, success));
        }
    }
}