using System;
using System.Linq;

using Quasar.Core;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.Items.Utilities;
using Quasar.HabboHotel.Catalog.Utilities;
using log4net;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class CatalogPageComposer : ServerPacket
    {
        public CatalogPageComposer(CatalogPage Page, string CataMode)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            base.WriteInteger(Page.Id);
            base.WriteString(CataMode);
            base.WriteString(Page.Template);

            base.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                base.WriteString(s);
            }

            base.WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                base.WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {
                /*if (Page.PredesignedItems != null)
                {
                    base.WriteInteger(Page.PredesignedItems.Items.Count);
                    foreach (var predesignedII in Page.PredesignedItems.Items)
                    {
                        var Item = Page.GetItem(Page.PredesignedItems.CatalogId);
                        base.WriteInteger(Item.Id);
                        base.WriteString(Item.Name);
                        base.WriteBoolean(false);
                        base.WriteInteger(Item.CostCredits);
                        if (Item.CostDiamonds > 0)
                        {
                            base.WriteInteger(Item.CostDiamonds);
                            base.WriteInteger(5); // Diamonds
                        }
                        else if (Item.CostGOTWPoints > 0)
                        {
                            base.WriteInteger(Item.CostGOTWPoints);
                            base.WriteInteger(103); // Puntos de Honor
                        }
                        else
                        {
                            base.WriteInteger(Item.CostPixels);
                            base.WriteInteger(0); // Type of PixelCost
                        }

                        base.WriteBoolean(false);
                        base.WriteInteger(Page.PredesignedItems.Items.Count);
                        Console.WriteLine("cnt: " + Page.PredesignedItems.Items.Count);
                        foreach (var predesigned in Page.PredesignedItems.Items)
                        {
                            var predesignedItem = Page.GetItem(predesigned.Key);
                            Console.WriteLine("ide: " + predesigned.Key);
                            base.WriteString(predesignedItem.Data.Type.ToString());
                            base.WriteInteger(predesignedItem.Data.SpriteId);
                            base.WriteString(string.Empty);
                            base.WriteInteger(predesigned.Value);//amount
                            base.WriteBoolean(false);
                        }

                        base.WriteInteger(0);
                        base.WriteBoolean(false);
                    }
                }*/
                /*else
                {*/
                base.WriteInteger(Page.Items.Count);
                foreach (var Item in Page.Items.Values)
                {
                    base.WriteInteger(Item.Id);
                    base.WriteString(Item.Name);
                    base.WriteBoolean(false);//IsRentable
                    base.WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
                        base.WriteInteger(Item.CostDiamonds);
                        base.WriteInteger(5); // Diamonds
                    }
                    else if (Item.CostGOTWPoints > 0)
                    {
                        base.WriteInteger(Item.CostGOTWPoints);
                        base.WriteInteger(103); // Puntos de Honor
                    }
                   
                    else if (Item.CostPumpkins > 0)
                    {
                        base.WriteInteger(Item.CostPumpkins);
                        base.WriteInteger(104); // Calabazas
                    }
                    
                    else
                    {
                        base.WriteInteger(Item.CostPixels);
                        base.WriteInteger(0); // Type of PixelCost
                    }

                    base.WriteBoolean(Item.PredesignedId > 0 ? false : ItemUtility.CanGiftItem(Item));
                    if (Item.PredesignedId > 0)
                    {
                        base.WriteInteger(Page.PredesignedItems.Items.Count);
                        foreach (var predesigned in Page.PredesignedItems.Items.ToList())
                        {
                            ItemData Data = null;
                            if (QuasarEnvironment.GetGame().GetItemManager().GetItem(predesigned.Key, out Data)) { }
                            base.WriteString(Data.Type.ToString());
                            base.WriteInteger(Data.SpriteId);
                            base.WriteString(string.Empty);
                            base.WriteInteger(predesigned.Value);
                            base.WriteBoolean(false);
                        }

                        base.WriteInteger(0);
                        base.WriteBoolean(false);
                        base.WriteBoolean(true); // Niu Rilí
                        base.WriteString(""); // Niu Rilí
                    }
                    else if (Page.Deals.Count > 0)
                    {
                        foreach (var Deal in Page.Deals.Values)
                        {
                            base.WriteInteger(Deal.ItemDataList.Count);
                            foreach (var DealItem in Deal.ItemDataList.ToList())
                            {
                                base.WriteString(DealItem.Data.Type.ToString());
                                base.WriteInteger(DealItem.Data.SpriteId);
                                base.WriteString(string.Empty);
                                base.WriteInteger(DealItem.Amount);
                                base.WriteBoolean(false);
                            }

                            base.WriteInteger(0);
                            base.WriteBoolean(false);
                        }
                    }
                    else
                    {
                        base.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.
                        {
                            if (!string.IsNullOrEmpty(Item.Badge))
                            {
                                base.WriteString("b");
                                base.WriteString(Item.Badge);
                            }

                            base.WriteString(Item.Data.Type.ToString());
                            if (Item.Data.Type.ToString().ToLower() == "b")
                            {
                                //This is just a badge, append the name.
                                base.WriteString(Item.Data.ItemName);
                            }
                            else
                            {
                                base.WriteInteger(Item.Data.SpriteId);
                                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                                {
                                    base.WriteString(Item.Name.Split('_')[2]);
                                }
                                else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                                {
                                    CatalogBot CatalogBot = null;
                                    if (!QuasarEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                                        base.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                    else
                                        base.WriteString(CatalogBot.Figure);
                                }
                                else if (Item.ExtraData != null)
                                {
                                    base.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                                }
                                base.WriteInteger(Item.Amount);
                                base.WriteBoolean(Item.IsLimited); // IsLimited
                                if (Item.IsLimited)
                                {
                                    base.WriteInteger(Item.LimitedEditionStack);
                                    base.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                                }
                            }
                            base.WriteInteger(0); //club_level
                            base.WriteBoolean(ItemUtility.CanSelectAmount(Item));

                            base.WriteBoolean(true); // Niu Rilí
                            base.WriteString(""); // Niu Rilí
                        }
                    }
                }
                /*}*/
            }
            else

                base.WriteInteger(0);
            base.WriteInteger(-1);
            base.WriteBoolean(false);

            if (Page.Template.Equals("frontpage4"))
            {

                ILog log = LogManager.GetLogger("Quasar.Core.ConsoleCommandHandler");
                try
                {
                    CatalogFrontPage frontpage = QuasarEnvironment.GetGame().getCatalogFrontPage();

                    var oneN = frontpage._oneN;
                    var twoN = frontpage._twoN;
                    var threeN = frontpage._threeN;
                    var fourN = frontpage._fourN;

                    var oneI = frontpage._oneI;
                    var twoI = frontpage._twoI;
                    var threeI = frontpage._threeI;
                    var fourI = frontpage._fourI;

                    var onePL = frontpage._onePL;
                    var twoPL = frontpage._twoPL;
                    var threePL = frontpage._threePL;
                    var fourPL = frontpage._fourPL;

                    base.WriteInteger(4);

                    base.WriteInteger(1);
                    base.WriteString(oneN);
                    base.WriteString(oneI);
                    base.WriteInteger(0);
                    base.WriteString(onePL);
                    base.WriteInteger(0);

                    base.WriteInteger(2);
                    base.WriteString(twoN);
                    base.WriteString(twoI);
                    base.WriteInteger(0);
                    base.WriteString(twoPL);
                    base.WriteInteger(0);

                    base.WriteInteger(3);
                    base.WriteString(threeN);
                    base.WriteString(threeI);
                    base.WriteInteger(0);
                    base.WriteString(threePL);
                    base.WriteInteger(0);

                    base.WriteInteger(4);
                    base.WriteString(fourN);
                    base.WriteString(fourI);
                    base.WriteInteger(0);
                    base.WriteString(fourPL);
                    base.WriteInteger(0);
                }
                catch (Exception e)
                {
                    string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
                    Console.WriteLine(CurrentTime + e.Message);

                    if (Page.Template.Equals("vip_buy"))
                    {
                        base.WriteInteger(0); //Page ID
                        base.WriteString("NORMAL");
                        base.WriteString("vip_buy");
                        base.WriteInteger(2);
                        base.WriteString("hc2_clubtitle");
                        base.WriteString("clubcat_pic");
                        base.WriteInteger(0); // Nueva Release
                        base.WriteInteger(0);
                        base.WriteInteger(-1);
                        base.WriteBoolean(false);

                        if (Page.Template.Equals("club_gifts"))
                        {
                            base.WriteString("club_gifts");
                            base.WriteInteger(1);
                            base.WriteString(Page.PageStrings2);
                            base.WriteInteger(1);
                            base.WriteString(Page.PageStrings2);
                        }
                    }
                }
            }
        }
    }
}