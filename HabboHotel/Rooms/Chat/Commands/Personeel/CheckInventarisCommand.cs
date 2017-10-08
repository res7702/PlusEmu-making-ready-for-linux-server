using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Personeel
{
    class CheckInventarisCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_checkinventaris"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Bekijk een andere Habbis zijn inventaris."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().ViewInventory)
            {
                Session.SendMessage(new FurniListComposer(Session.GetHabbo().GetInventoryComponent().GetFloorItems().ToList(), Session.GetHabbo().GetInventoryComponent().GetWallItems()));
                Session.GetHabbo().ViewInventory = false;
                Session.SendWhisper("Je inventaris is terug naar die van jezelf gegaan.");
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je bent vergeten om een gebruikersnaam in te voeren.");
                return;
            }

            GameClient TargetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oeps! Deze Habbis bestaat niet of is niet in de kamer aanwezig.");
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Oeps! Deze Habbis bestaat niet of is niet in de kamer aanwezig.");
                return;
            }

            //Session.SendMessage(new FurniListComposer(TargetClient.GetHabbo().GetInventoryComponent().GetFloorItems().ToList()));
            Session.GetHabbo().ViewInventory = true;

            Session.SendWhisper("Open de inventaris om al de items van " + TargetClient.GetHabbo().Username + " te kunnen bekijken.");
        }
    }
}