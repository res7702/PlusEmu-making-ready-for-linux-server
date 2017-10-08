using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Quasar.Database.Interfaces;


namespace Quasar.HabboHotel.Catalog.Pets
{
    public class PetRaceManager
    {
        private List<PetRace> _races = new List<PetRace>();

        public void Init()
        {
            if (this._races.Count > 0)
                this._races.Clear();

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `catalog_pet_races`");
                DataTable Table = dbClient.getTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        PetRace Race = new PetRace(Convert.ToInt32(Row["raceid"]), Convert.ToInt32(Row["color1"]), Convert.ToInt32(Row["color2"]), (Convert.ToString(Row["has1color"]) == "1"), (Convert.ToString(Row["has2color"]) == "1"));
                        if (!this._races.Contains(Race))
                            this._races.Add(Race);
                    }
                }
            }
        }

        public int GetPetId(string Type, out string Packet)
        {
            int PetId = 0;
            Packet = "";

            switch (Type)
            {
                // Honden
                // SWF: dog
                case "a0 pet0":
                    Packet = "a0 pet0";
                    PetId = 0;
                    break;

                // Katten
                // SWF: cat
                case "a0 pet1":
                    Packet = "a0 pet1";
                    PetId = 1;
                    break;

                // Krokodillen
                // SWF: croco
                case "a0 pet2":
                    Packet = "a0 pet2";
                    PetId = 2;
                    break;

                // Terriers
                // SWF: terrier
                case "a0 pet3":
                    Packet = "a0 pet3";
                    PetId = 3;
                    break;

                // Beren
                // SWF: bear
                case "a0 pet4":
                    Packet = "a0 pet4";
                    PetId = 4;
                    break;

                // Varkens
                // SWF: pig
                case "a0 pet5":
                    Packet = "a0 pet5";
                    PetId = 5;
                    break;

                // Leeuwen
                // SWF: lion
                case "a0 pet6":
                    Packet = "a0 pet6";
                    PetId = 6;
                    break;

                // Neushoorns
                // SWF: rhino
                case "a0 pet7":
                    Packet = "a0 pet7";
                    PetId = 7;
                    break;

                // Spinnen
                // SWF: spider
                case "a0 pet8":
                    Packet = "a0 pet8";
                    PetId = 8;
                    break;

                // Schildpadden
                // SWF: turtle
                case "a0 pet9":
                    Packet = "a0 pet9";
                    PetId = 9;
                    break;

                // Kuikens
                // SWF: chicken
                case "a0 pet10":
                    Packet = "a0 pet10";
                    PetId = 10;
                    break;

                // Kikkers
                // SWF: frog
                case "a0 pet11":
                    Packet = "a0 pet11";
                    PetId = 11;
                    break;

                // Draken
                // SWF: dragon
                case "a0 pet12":
                    Packet = "a0 pet12";
                    PetId = 12;
                    break;

                // Slenderman
                // SWF: slendermn
                case "a0 pet13":
                    Packet = "a0 pet13";
                    PetId = 13;
                    break;

                // Apen
                // SWF: monkey
                case "a0 pet14":
                    Packet = "a0 pet14";
                    PetId = 14;
                    break;

                // Paarden
                // SWF: horse
                case "a0 pet15":
                    Packet = "a0 pet15";
                    PetId = 15;
                    break;

                // Monsterplanten
                // SWF: monsterplant
                case "a0 pet16":
                    Packet = "a0 pet16";
                    PetId = 16;
                    break;

                // Konijnen
                // SWF: bunnyeaster
                case "a0 pet17":
                    Packet = "a0 pet17";
                    PetId = 17;
                    break;

                // Evil Konijnen
                // SWF: bunnyevil
                case "a0 pet18":
                    Packet = "a0 pet18";
                    PetId = 18;
                    break;

                // Depressieve Konijnen
                // SWF: bunnydepressed
                case "a0 pet19":
                    Packet = "a0 pet19";
                    PetId = 19;
                    break;

                // Liefdes Konijnen
                // SWF: bunnylove
                case "a0 pet20":
                    Packet = "a0 pet20";
                    PetId = 20;
                    break;

                // Witte Duiven
                // SWF: pigeongood
                case "a0 pet21":
                    Packet = "a0 pet21";
                    PetId = 21;
                    break;

                // Zwarte Duiven
                // SWF: pigeonevil
                case "a0 pet22":
                    Packet = "a0 pet22";
                    PetId = 22;
                    break;

                // Rode Apen
                // SWF: demonmonkey
                case "a0 pet23":
                    Packet = "a0 pet23";
                    PetId = 23;
                    break;

                // Baby Beren
                // SWF: bearbaby
                case "a0 pet24":
                    Packet = "a0 pet24";
                    PetId = 24;
                    break;

                // Baby Terriers
                // SWF: terrierbaby
                case "a0 pet25":
                    Packet = "a0 pet25";
                    PetId = 25;
                    break;

                // Kabouters
                // SWF: gnome
                case "a0 pet26":
                    Packet = "a0 pet26";
                    PetId = 26;
                    break;

                // Baby's 
                // SWF: babyBH
                case "a0 pet27":
                    Packet = "a0 pet27";
                    PetId = 27;
                    break;

                // Baby Kittens
                // SWF: kittenbaby
                case "a0 pet28":
                    Packet = "a0 pet28";
                    PetId = 28;
                    break;

                // Baby Puppy's
                // SWF: puppybaby
                case "a0 pet29":
                    Packet = "a0 pet29";
                    PetId = 29;
                    break;

                // Baby Varkens
                // SWF: pigletbaby
                case "a0 pet30":
                    Packet = "a0 pet30";
                    PetId = 30;
                    break;

                // Oempa Loempa's
                // SWF: haloompa
                case "a0 pet31":
                    Packet = "a0 pet31";
                    PetId = 31;
                    break;

                // Stenen
                // SWF: fools
                case "a0 pet32":
                    Packet = "a0 pet32";
                    PetId = 32;
                    break;

                // Pterodactylussen
                // SWF: pterosaur
                case "a0 pet33":
                    Packet = "a0 pet33";
                    PetId = 33;
                    break;

                // Velociraptors 
                // SWF: velociraptor
                case "a0 pet34":
                    Packet = "a0 pet34";
                    PetId = 34;
                    break;

                // Wolven
                // SWF: wolfbabe
                case "a0 pet35":
                    Packet = "a0 pet35";
                    PetId = 35;
                    break;

                // Monster Konijnen
                // SWF: bunnymnster
                case "a0 pet36":
                    Packet = "a0 pet36";
                    PetId = 36;
                    break;

                // Pickachu Pokemon
                // SWF: pickahupet1
                case "a0 pet37": // 
                    Packet = "a0 pet37";
                    PetId = 37;
                    break;

                // Pinguins
                // SWF: penguin
                case "a0 pet38":
                    Packet = "a0 pet38";
                    PetId = 38;
                    break;

                // Mario
                // SWF: mario_viter_hh
                case "a0 pet39":
                    Packet = "a0 pet39";
                    PetId = 39;
                    break;

                // Olifanten
                // SWF: elephants
                case "a0 pet40":
                    Packet = "a0 pet40";
                    PetId = 40;
                    break;

                // Alien Konijn
                // SWF: bunny_alien_hd
                case "a0 pet41":
                    Packet = "a0 pet41";
                    PetId = 41;
                    break;

                // Gouden Konijnen
                // SWF: bunnytdid01
                case "a0 pet42":
                    Packet = "a0 pet42";
                    PetId = 42;
                    break;

                // Pokémon: Mew
                // SWF: pokmn_mew
                case "a0 pet43":
                    Packet = "a0 pet43";
                    PetId = 43;
                    break;

                // Pokémon: Entei
                // SWF: pkmentei
                case "a0 pet44":
                    Packet = "a0 pet44";
                    PetId = 44;
                    break;

                // Pokémon: Mew
                // SWF: pokemon_mewblue
                case "a0 pet45":
                    Packet = "a0 pet45";
                    PetId = 45;
                    break;

                // Cavia
                // SWF: guineapig12
                case "a0 pet46":
                    Packet = "a0 pet46";
                    PetId = 46;
                    break;

                // Uil
                // SWF: Bw_owl1
                case "a0 pet47":
                    Packet = "a0 pet47";
                    PetId = 47;
                    break;

                // Pokémon: Mew
                // SWF: legendmew
                case "a0 pet48":
                    Packet = "a0 pet48";
                    PetId = 48;
                    break;

                // Gereseveerd
                // SWF: ducky123456
                case "a0 pet49":
                    Packet = "a0 pet49";
                    PetId = 49;
                    break;

                // Bruine Baby
                // SWF: blackb
                case "a0 pet50":
                    Packet = "a0 pet50";
                    PetId = 50;
                    break;

                // Witte Baby
                // SWF: bbwibb
                case "a0 pet51":
                    Packet = "a0 pet51";
                    PetId = 51;
                    break;

                // Dino Ei
                // SWF: DinoE
                case "a0 pet52":
                    Packet = "a0 pet52";
                    PetId = 52;
                    break;

                // Witte Baby
                // SWF: bbwibb
                case "a0 pet53":
                    Packet = "a0 pet53";
                    PetId = 53;
                    break;

                // Koe
                // SWF: cow
                case "a0 pet54":
                    Packet = "a0 pet54";
                    PetId = 54;
                    break;

                // Pokémon: Gengar
                // SWF: pkmgengarpe
                case "a0 pet55":
                    Packet = "a0 pet55";
                    PetId = 55;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet56":
                    Packet = "a0 pet56";
                    PetId = 56;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet57":
                    Packet = "a0 pet57";
                    PetId = 57;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet58":
                    Packet = "a0 pet58";
                    PetId = 58;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet59":
                    Packet = "a0 pet59";
                    PetId = 59;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet60":
                    Packet = "a0 pet60";
                    PetId = 60;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet61":
                    Packet = "a0 pet61";
                    PetId = 61;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet62":
                    Packet = "a0 pet62";
                    PetId = 62;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet63":
                    Packet = "a0 pet63";
                    PetId = 63;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet64":
                    Packet = "a0 pet64";
                    PetId = 64;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet65":
                    Packet = "a0 pet65";
                    PetId = 65;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet66":
                    Packet = "a0 pet66";
                    PetId = 66;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet67":
                    Packet = "a0 pet67";
                    PetId = 67;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet68":
                    Packet = "a0 pet68";
                    PetId = 68;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet69":
                    Packet = "a0 pet69";
                    PetId = 69;
                    break;

                // Pokémon: Reserved
                // SWF: Reserved
                case "a0 pet70":
                    Packet = "a0 pet70";
                    PetId = 70;
                    break;
            }
        
            return PetId;
        }

        public List<PetRace> GetRacesForRaceId(int RaceId)
        {
            return this._races.Where(Race => Race.RaceId == RaceId).ToList();
        }
    }
}