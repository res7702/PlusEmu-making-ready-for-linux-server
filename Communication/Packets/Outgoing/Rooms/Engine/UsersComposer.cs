using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Groups;
using System.Collections.ObjectModel;
using Quasar.HabboHotel.Rooms.AI;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Engine
{
    class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomUser> Users)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            base.WriteInteger(Users.Count);
            foreach (RoomUser User in Users.ToList())
            {
                WriteUser(User);
            }
        }

        public UsersComposer(RoomUser User)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            base.WriteInteger(1);//1 avatar
            WriteUser(User);
        }

        private void WriteUser(RoomUser User)
        {
            if (!User.IsPet && !User.IsBot)
            {
                Habbo Habbo = User.GetClient().GetHabbo();

                Group Group = null;
                if (Habbo != null)
                {
                    if (Habbo.GetStats() != null)
                    {
                        if (Habbo.GetStats().FavouriteGroupId > 0)
                        {
                            if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(Habbo.GetStats().FavouriteGroupId, out Group))
                                Group = null;
                        }
                    }
                }

                if (Habbo.PetId == 0)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(Habbo.Look);
                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);

                    base.WriteInteger(0);//2 for user, 4 for bot.
                    base.WriteInteger(1);//1 for user, 2 for pet, 3 for bot.
                    base.WriteString(Habbo.Gender.ToLower());

                    if (Group != null)
                    {
                        base.WriteInteger(Group.Id);
                        base.WriteInteger(0);
                        base.WriteString(Group.Name);
                    }
                    else
                    {
                        base.WriteInteger(0);
                        base.WriteInteger(0);
                        base.WriteString("");
                    }

                    base.WriteString("");//Whats this?
                    base.WriteInteger(Habbo.GetStats().AchievementPoints);//Achievement score
                    base.WriteBoolean(false);//Builders club?
                }
                else if (Habbo.PetId > 0 && Habbo.PetId != 100)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(PetFigureForType(Habbo.PetId));

                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);
                    base.WriteInteger(0);
                    base.WriteInteger(2);//Pet.

                    base.WriteInteger(Habbo.PetId);//pet type.
                    base.WriteInteger(Habbo.Id);//UserId of the owner.
                    base.WriteString(Habbo.Username);//Username of the owner.
                    base.WriteInteger(1);
                    base.WriteBoolean(false);//Has saddle.
                    base.WriteBoolean(false);//Is someone riding this horse?
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteString("");
                }
                else if (Habbo.PetId > 0 && Habbo.PetId == 100)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(Habbo.Look.ToLower());
                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);
                    base.WriteInteger(0);
                    base.WriteInteger(4);

                    base.WriteString(Habbo.Gender.ToLower()); // ?
                    base.WriteInteger(Habbo.Id); //Owner Id
                    base.WriteString(Habbo.Username); // Owner name
                    base.WriteInteger(0);//Action Count
                }
            }
            else if (User.IsPet)
            {
                base.WriteInteger(User.BotAI.BaseId);
                base.WriteString(User.BotData.Name);
                base.WriteString(User.BotData.Motto);

                //base.WriteString("26 30 ffffff 5 3 302 4 2 201 11 1 102 12 0 -1 28 4 401 24");
                base.WriteString(User.BotData.Look.ToLower() + ((User.PetData.Saddle > 0) ? " 3 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 4 " + User.PetData.Saddle + " 0" : " 2 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + ""));

                base.WriteInteger(User.VirtualId);
                base.WriteInteger(User.X);
                base.WriteInteger(User.Y);
                base.WriteDouble(User.Z);
                base.WriteInteger(0);
                base.WriteInteger((User.BotData.AiType == BotAIType.PET) ? 2 : 4);
                base.WriteInteger(User.PetData.Type);
                base.WriteInteger(User.PetData.OwnerId); // userid
                base.WriteString(User.PetData.OwnerName); // username
                base.WriteInteger(1);
                base.WriteBoolean(User.PetData.Saddle > 0);
                base.WriteBoolean(User.RidingHorse);
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteString("");
            }
            else if (User.IsBot)
            {
                base.WriteInteger(User.BotAI.BaseId);
                base.WriteString(User.BotData.Name);
                base.WriteString(User.BotData.Motto);
                base.WriteString(User.BotData.Look.ToLower());
                base.WriteInteger(User.VirtualId);
                base.WriteInteger(User.X);
                base.WriteInteger(User.Y);
                base.WriteDouble(User.Z);
                base.WriteInteger(0);
                base.WriteInteger((User.BotData.AiType == BotAIType.PET) ? 2 : 4);

                base.WriteString(User.BotData.Gender.ToLower()); // ?
                base.WriteInteger(User.BotData.ownerID); //Owner Id
                base.WriteString(QuasarEnvironment.GetUsernameById(User.BotData.ownerID)); // Owner name
                base.WriteInteger(5);//Action Count
                base.WriteShort(1);//Copy looks
                base.WriteShort(2);//Setup speech
                base.WriteShort(3);//Relax
                base.WriteShort(4);//Dance
                base.WriteShort(5);//Change name
            }
        }

        public string PetFigureForType(int Type)
        {
            Random _random = new Random();

            switch (Type)
            {
                #region 0 Honden
                default:
                case 60:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "0 0 f08b90 2 2 -1 1 3 -1 1";
                            case 2:
                                return "0 15 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "0 20 d98961 2 2 -1 0 3 -1 0";
                            case 4:
                                return "0 21 da9dbd 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 1 Katten
                case 1:
                    {
                        int RandomNumber = _random.Next(1, 5);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "1 18 d5b35f 2 2 -1 0 3 -1 0";
                            case 2:
                                return "1 0 ff7b3a 2 2 -1 0 3 -1 0";
                            case 3:
                                return "1 18 d98961 2 2 -1 0 3 -1 0";
                            case 4:
                                return "1 0 ff7b3a 2 2 -1 0 3 -1 1";
                            case 5:
                                return "1 24 d5b35f 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 2 Terriers
                case 2:
                    {
                        int RandomNumber = _random.Next(1, 6);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "3 3 eeeeee 2 2 -1 0 3 -1 0";
                            case 2:
                                return "3 0 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "3 5 eeeeee 2 2 -1 0 3 -1 0";
                            case 4:
                                return "3 6 eeeeee 2 2 -1 0 3 -1 0";
                            case 5:
                                return "3 4 dddddd 2 2 -1 0 3 -1 0";
                            case 6:
                                return "3 5 dddddd 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 3 Krokodillen
                case 3:
                    {
                        int RandomNumber = _random.Next(1, 5);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "2 10 84ce84 2 2 -1 0 3 -1 0";
                            case 2:
                                return "2 8 838851 2 2 0 0 3 -1 0";
                            case 3:
                                return "2 11 b99105 2 2 -1 0 3 -1 0";
                            case 4:
                                return "2 3 e8ce25 2 2 -1 0 3 -1 0";
                            case 5:
                                return "2 2 fcfad3 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 4 Beren
                case 4:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "4 2 e4feff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "4 3 e4feff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "4 1 eaeddf 2 2 -1 0 3 -1 0";
                            case 4:
                                return "4 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 5 Varkens
                case 5:
                    {
                        int RandomNumber = _random.Next(1, 7);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "5 2 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "5 0 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "5 3 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "5 5 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "5 7 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "5 1 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "5 8 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 6 Leeuwen
                case 6:
                    {
                        int RandomNumber = _random.Next(1, 11);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "6 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "6 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "6 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "6 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "6 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "6 0 ffd8c9 2 2 -1 0 3 -1 0";
                            case 7:
                                return "6 5 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "6 11 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "6 2 ffe49d 2 2 -1 0 3 -1 0";
                            case 10:
                                return "6 11 ff9ae 2 2 -1 0 3 -1 0";
                            case 11:
                                return "6 2 ff9ae 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 7 Neushoorns
                case 7:
                    {
                        int RandomNumber = _random.Next(1, 7);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "7 5 aeaeae 2 2 -1 0 3 -1 0";
                            case 2:
                                return "7 7 ffc99a 2 2 -1 0 3 -1 0";
                            case 3:
                                return "7 5 cccccc 2 2 -1 0 3 -1 0";
                            case 4:
                                return "7 5 9adcff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "7 5 ff7d6a 2 2 -1 0 3 -1 0";
                            case 6:
                                return "7 6 cccccc 2 2 -1 0 3 -1 0";
                            case 7:
                                return "7 0 cccccc 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 8 Spinnen
                case 8:
                    {
                        int RandomNumber = _random.Next(1, 13);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "8 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "8 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "8 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "8 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "8 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "8 14 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "8 11 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "8 8 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "8 6 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "8 5 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "8 9 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "8 10 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "8 7 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region  9 Schildpadden
                case 9:
                    {
                        int RandomNumber = _random.Next(1, 9);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "9 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "9 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "9 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "9 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "9 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "9 5 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "9 6 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "9 7 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "9 8 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 10 Kuikens
                case 10:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "10 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 11 Kikkers
                case 11:
                    {
                        int RandomNumber = _random.Next(1, 13);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "11 1 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "11 2 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "11 3 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "11 4 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "11 5 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "11 9 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "11 10 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "11 6 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "11 12 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "11 11 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "11 15 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "11 13 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "11 18 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                    #endregion
               
                #region 12 Draken
                case 12:
                    {
                        int RandomNumber = _random.Next(1, 6);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "12 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "12 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "12 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "12 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "12 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "12 5 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 13 Slendermen
                case 13:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "13 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 14 Apen
                case 14:
                    {
                        int RandomNumber = _random.Next(1, 14);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "14 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "14 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "14 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "14 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "14 6 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "14 4 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "14 5 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "14 7 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "14 8 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "14 9 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "14 10 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "14 11 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "14 12 ffffff 2 2 -1 0 3 -1 0";
                            case 14:
                                return "14 13 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 15 Paarden
                case 15:
                    {
                        int RandomNumber = _random.Next(1, 20);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "15 2 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "15 3 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "15 4 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "15 5 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "15 6 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "15 7 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "15 8 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "15 9 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "15 10 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "15 11 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "15 12 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "15 13 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "15 14 ffffff 2 2 -1 0 3 -1 0";
                            case 14:
                                return "15 15 ffffff 2 2 -1 0 3 -1 0";
                            case 15:
                                return "15 16 ffffff 2 2 -1 0 3 -1 0";
                            case 16:
                                return "15 17 ffffff 2 2 -1 0 3 -1 0";
                            case 17:
                                return "15 78 ffffff 2 2 -1 0 3 -1 0";
                            case 18:
                                return "15 77 ffffff 2 2 -1 0 3 -1 0";
                            case 19:
                                return "15 79 ffffff 2 2 -1 0 3 -1 0";
                            case 20:
                                return "15 80 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 16 Monsterplant
                case 16:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "16 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region 17 Konijnen
                case 17:
                    {
                        int RandomNumber = _random.Next(1, 8);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "17 1 ffffff";
                            case 2:
                                return "17 2 ffffff";
                            case 3:
                                return "17 3 ffffff";
                            case 4:
                                return "17 4 ffffff";
                            case 5:
                                return "17 5 ffffff";
                            case 6:
                                return "18 0 ffffff";
                            case 7:
                                return "19 0 ffffff";
                            case 8:
                                return "20 0 ffffff";
                        }
                    }
                #endregion

                #region 18 Konijn Slechtaardig
                case 18:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "17 1 ffffff";
                        }
                    }
                #endregion

                #region 19 Konijn Depressief
                case 19:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "19 1 ffffff";
                        }
                    }
                #endregion

                #region 20 Konijn Zachtaardig
                case 20:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "20 1 ffffff";
                        }
                    }
                #endregion

                #region 21 Duif Goedaardig
                case 21:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "21 0 ffffff";
                        }
                    }
                #endregion

                #region 22 Duif Slechtaardig
                case 22:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "22 0 ffffff";
                        }
                    }
                #endregion

                #region 23 Demoon Apen
                case 23:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "23 0 ffffff";
                            case 2:
                                return "23 1 ffffff";
                            case 3:
                                return "23 3 ffffff";
                        }
                    }
                #endregion

                #region 24 Beer Puppy
                case 24:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "24 1 ffffff";
                        }
                    }
                #endregion

                #region 25 Terriërs Puppy
                case 25:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "25 1 ffffff";
                        }
                    }
                #endregion

                #region 26 Dwergen
                case 26:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "26 1 ffffff 5 0 -1 0 4 402 5 3 301 4 1 101 2 2 201 3";
                            case 2:
                                return "26 1 ffffff 5 0 -1 0 1 102 13 3 301 4 4 401 5 2 201 3";
                            case 3:
                                return "26 6 ffffff 5 1 102 8 2 201 16 4 401 9 3 303 4 0 -1 6";
                            case 4:
                                return "26 30 ffffff 5 0 -1 0 3 303 4 4 401 5 1 101 2 2 201 3";
                        }
                    }
                #endregion

                #region 27 Baby 3
                case 27:
                    {
                        int RandomNumber = _random.Next(1, 9);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "27 0 ffffff";
                            case 2:
                                return "27 1 ffffff";
                            case 3:
                                return "27 2 ffffff";
                            case 4:
                                return "27 3 ffffff";
                            case 5:
                                return "27 4 ffffff";
                            case 6:
                                return "27 5 ffffff";
                            case 7:
                                return "27 6 ffffff";
                            case 8:
                                return "27 7 ffffff";
                            case 9:
                                return "27 8 ffffff";
                        }
                    }
                #endregion

                #region 28 Kittens
                case 28:
                    {
                        int RandomNumber = _random.Next(1, 10);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "28 1 ffffff";
                            case 2:
                                return "28 2 ffffff";
                            case 3:
                                return "28 3 ffffff";
                            case 4:
                                return "28 4 ffffff";
                            case 5:
                                return "28 5 ffffff";
                            case 6:
                                return "28 6 ffffff";
                            case 7:
                                return "28 7 ffffff";
                            case 8:
                                return "28 8 ffffff";
                            case 9:
                                return "28 9 ffffff";
                            case 10:
                                return "28 10 ffffff";
                        }
                    }
                #endregion

                #region 29 Honden Puppy
                case 29:
                    {
                        int RandomNumber = _random.Next(1, 10);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "29 1 ffffff";
                            case 2:
                                return "29 2 ffffff";
                            case 3:
                                return "29 3 ffffff";
                            case 4:
                                return "29 4 ffffff";
                            case 5:
                                return "29 5 ffffff";
                            case 6:
                                return "29 6 ffffff";
                            case 7:
                                return "29 7 ffffff";
                            case 8:
                                return "29 8 ffffff";
                            case 9:
                                return "29 9 ffffff";
                            case 10:
                                return "29 10 ffffff";
                        }
                    }
                #endregion

                #region 30 Varkens Puppy
                case 30:
                    {
                        int RandomNumber = _random.Next(1, 10);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "30 1 ffffff";
                            case 2:
                                return "30 2 ffffff";
                            case 3:
                                return "30 3 ffffff";
                            case 4:
                                return "30 4 ffffff";
                            case 5:
                                return "30 5 ffffff";
                            case 6:
                                return "30 6 ffffff";
                            case 7:
                                return "30 7 ffffff";
                            case 8:
                                return "30 8 ffffff";
                            case 9:
                                return "30 9 ffffff";
                            case 10:
                                return "30 10 ffffff";
                        }
                    }
                #endregion

                #region 31 Haloompa's
                case 31:
                    {
                        int RandomNumber = _random.Next(1, 10);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "29 0 ffffff";
                            case 2:
                                return "29 1 ffffff";
                            case 3:
                                return "29 2 ffffff";
                            case 4:
                                return "29 3 ffffff";
                            case 5:
                                return "29 4 ffffff";
                            case 6:
                                return "29 5 ffffff";
                            case 7:
                                return "29 6 ffffff";
                            case 8:
                                return "29 7 ffffff";
                            case 9:
                                return "29 8 ffffff";
                            case 10:
                                return "29 9 ffffff";
                        }
                    }
                #endregion

                #region 32 Steen
                case 32:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "32 0 ffffff";
                        }
                    }
                #endregion

                #region 33 Pterodactylussen
                case 33:
                    {
                        int RandomNumber = _random.Next(1, 20);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "33 0 ffffff";
                            case 2:
                                return "33 1 ffffff";
                            case 3:
                                return "33 2 ffffff";
                            case 4:
                                return "33 3 ffffff";
                            case 5:
                                return "33 4 ffffff";
                            case 6:
                                return "33 5 ffffff";
                            case 7:
                                return "33 6 ffffff";
                            case 8:
                                return "33 7 ffffff";
                            case 9:
                                return "33 8 ffffff";
                            case 10:
                                return "33 9 ffffff";
                            case 11:
                                return "33 10 ffffff";
                            case 12:
                                return "33 11 ffffff";
                            case 13:
                                return "33 12 ffffff";
                            case 14:
                                return "33 13 ffffff";
                            case 15:
                                return "33 14 ffffff";
                            case 16:
                                return "33 15 ffffff";
                            case 17:
                                return "33 16 ffffff";
                            case 18:
                                return "33 17 ffffff";
                            case 19:
                                return "33 18 ffffff";
                            case 20:
                                return "33 19 ffffff";
                        }
                    }
                #endregion

                #region 34 Velociraptors
                case 34:
                    {
                        int RandomNumber = _random.Next(1, 20);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "34 0 ffffff";
                            case 2:
                                return "34 1 ffffff";
                            case 3:
                                return "34 2 ffffff";
                            case 4:
                                return "34 3 ffffff";
                            case 5:
                                return "34 4 ffffff";
                            case 6:
                                return "34 5 ffffff";
                            case 7:
                                return "34 6 ffffff";
                            case 8:
                                return "34 7 ffffff";
                            case 9:
                                return "34 8 ffffff";
                            case 10:
                                return "34 9 ffffff";
                            case 11:
                                return "34 10 ffffff";
                            case 12:
                                return "34 11 ffffff";
                            case 13:
                                return "34 12 ffffff";
                            case 14:
                                return "34 13 ffffff";
                            case 15:
                                return "34 14 ffffff";
                            case 16:
                                return "34 15 ffffff";
                            case 17:
                                return "34 16 ffffff";
                            case 18:
                                return "34 17 ffffff";
                            case 19:
                                return "34 18 ffffff";
                            case 20:
                                return "34 19 ffffff";
                        }
                    }
                #endregion

                #region 35 Wolf
                case 35:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "35 40 ffffff";
                        }
                    }
                #endregion

                #region 36 Konijn Spookachtig
                case 36:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "36 0 ffffff";
                        }
                    }
                #endregion

                #region 37 Pickachu
                case 37:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "37 0 ffffff";
                        }
                    }
                #endregion

                #region 38 Pinguins
                case 38:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "38 0 ffffff";
                        }
                    }
                #endregion

                #region 39 Mario
                case 39:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "39 0 ffffff";
                        }
                    }
                #endregion

                #region 40 Olifanten
                case 40:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "40 0 ffffff";
                        }
                    }
                #endregion

                #region 41 Konijn Monsterlijk
                case 41:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "41 0 ffffff";
                        }
                    }
                #endregion

                #region 42 Konijn Goudkleurig
                case 42:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "42 0 ffffff";
                        }
                    }
                #endregion

                #region 43 Mewtwo Roze
                case 43:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "43 1 ffffff";
                        }
                    }
                #endregion

                #region 44 Entei
                case 44:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "44 1 ffffff";
                        }
                    }
                #endregion

                #region 45 Mewtwo Blauw
                case 45:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "45 1 ffffff";
                        }
                    }
                #endregion

                #region 46 Cavia
                case 46:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "46 50 ffffff";
                        }
                    }
                #endregion

                #region 47 Uil
                case 47:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "47 1 ffffff";
                        }
                    }
                #endregion

                #region 48 Mewtwo Goud
                case 48:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "48 1 ffffff";
                        }
                    }
                #endregion

                #region 49 Eend
                case 49:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "49 1 ffffff";
                        }
                    }
                #endregion

                #region 50 Baby 2
                case 50:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "50 1 ffffff";
                        }
                    }
                #endregion

                #region 51 Baby 1
                case 51:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "51 1 ffffff";
                        }
                    }
                    #endregion
            }
        }
    }
}