using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.Utilities;
using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Encryption;
using Quasar.Communication.Encryption.Crypto.Prng;
using Quasar.Communication.Packets.Outgoing.Handshake;

namespace Quasar.Communication.Packets.Incoming.Handshake
{
    public class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string CipherPublickey = Packet.PopString();
           
            BigInteger SharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(CipherPublickey);
            if (SharedKey != 0)
            {
                Session.RC4Client = new ARC4(SharedKey.getBytes());
                Session.SendMessage(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
            }
            else 
            {
                Session.SendNotification("Oeps! Er is een fout opgetreden.");
                return;
            }
        }
    }
}