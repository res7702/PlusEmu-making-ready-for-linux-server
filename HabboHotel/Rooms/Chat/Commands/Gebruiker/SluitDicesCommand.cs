using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Gebruiker
{
    class SluitDicesCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_sluit_dices"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Sluit de vijf open dices om je heen."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser roomUser = Room?.GetRoomUserManager()?.GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (roomUser == null)
            {
                return;
            }

            List<Items.Item> userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
                x.Coordinate, roomUser.Point) && x.Data.InteractionType == Items.InteractionType.DICE).ToList();

            if (userBooth.Count != 5)
            {
                Session.SendWhisper("Oeps! Er zijn helaas geen vijf dices gevonden.");
                return;
            }

            userBooth.ForEach(x => {
                x.ExtraData = "0";
                x.UpdateState();
            });

            Session.SendWhisper("Hoppa! De dices zijn gesloten.");
        }
    }
}