using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms.AI;
using Quasar.Communication.Packets.Incoming;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();
            string PacketType = "";
            int PetId = QuasarEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetPetId(Type, out PacketType);

            Session.SendMessage(new SellablePetBreedsComposer(PacketType, PetId, QuasarEnvironment.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(PetId)));
        }
    }
}