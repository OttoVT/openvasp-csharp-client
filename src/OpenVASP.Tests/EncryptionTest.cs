using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using OpenVASP.CSharpClient.Cryptography;
using Xunit;
using Xunit.Abstractions;

namespace OpenVASP.Tests
{
    public class EncryptionTest
    {
        private readonly ITestOutputHelper testOutputHelper;

        public EncryptionTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SharedSecretGenerationTest()
        {
            X25519Key alice = X25519Key.GenerateKey();
            X25519Key bob = X25519Key.GenerateKey();

            var shared1 = alice.GenerateSharedSecretHex(bob.PublicKey);
            var shared2 = bob.GenerateSharedSecretHex(alice.PublicKey);

            Assert.Equal(shared1, shared2);

            testOutputHelper.WriteLine(shared1);
            testOutputHelper.WriteLine(shared2);
        }

        [Fact]
        public void SharedSecretGenerationBasedOnEthKeyImportTest()
        {
            EthECKey ecKey1 = EthECKey.GenerateKey();
            EthECKey ecKey2 = EthECKey.GenerateKey();
            X25519Key alice = X25519Key.ImportKey(ecKey1.GetPrivateKey());
            X25519Key bob = X25519Key.ImportKey(ecKey2.GetPrivateKey());
            
            var s1 = alice.GenerateSharedSecretHex(ecKey2.GetPubKey().ToHex(true));
            var s2 = bob.GenerateSharedSecretHex(ecKey1.GetPubKey().ToHex(true));

            var shared1 = alice.GenerateSharedSecretHex(bob.PublicKey);
            var shared2 = bob.GenerateSharedSecretHex(alice.PublicKey);

            Assert.Equal(shared1, shared2);

            testOutputHelper.WriteLine(shared1);
            testOutputHelper.WriteLine(shared2);
        }
    }
}
