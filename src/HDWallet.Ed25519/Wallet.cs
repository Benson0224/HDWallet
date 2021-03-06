using System;
using HDWallet.Core;
using Ed25519;

namespace HDWallet.Ed25519
{
    public class Wallet : IWallet
    {
        byte[] privateKey;
        
        public byte[] PrivateKey {
            get {
                return privateKey;
            } set{
                if(value.Length != 32)
                {
                    throw new InvalidOperationException();
                }
                privateKey = value;

                ReadOnlySpan<byte> privateKeySpan = privateKey.AsSpan();
                var publicKey = privateKeySpan.ExtractPublicKey();

                PublicKey = publicKey.ToArray();
            }
        }
        public byte[] PublicKey;
        public uint Index;
        public string Address => AddressGenerator.GenerateAddress(PublicKey);

        public IAddressGenerator AddressGenerator;
        
        public Wallet(){}

        public Signature Sign(byte[] message)
        {
            if (message.Length != 32) throw new ArgumentException(paramName: nameof(message), message: "Message should be 32 bytes");

            var signature = Signer.Sign(message, this.PrivateKey, this.PublicKey);
            var signatureHex = signature.ToArray().ToHexString();

            
            var rsigPad = new byte[32];
            Array.Copy(signature.ToArray(), 0, rsigPad, rsigPad.Length - 32, 32);

            var ssigPad = new byte[32];
            Array.Copy(signature.ToArray(), 32, ssigPad, ssigPad.Length - 32, 32);

            return new Signature()
            {
                R = rsigPad,
                S = ssigPad
            };
        }
    }
}