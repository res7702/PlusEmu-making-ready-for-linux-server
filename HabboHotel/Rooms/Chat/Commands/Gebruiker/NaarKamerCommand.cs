using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class NaarKamerCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_naarkamer"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ga naar een kamer toe (id)."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oeps! Je moet een specifiek kamer id invullen!");
                return;
            }

            int RoomID;

            if (!int.TryParse(Params[1], out RoomID))
                Session.SendWhisper("Oeps! Je moet een correct kamer id invullen.");
            else
            {
                Room _room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(RoomID);
                if (_room == null)
                    Session.SendWhisper("Oeps! Dit kamer id bestaat niet!");
                else
                {
                    Session.GetHabbo().PrepareRoom(_room.Id, "");
                }
            }
        }
    }
}