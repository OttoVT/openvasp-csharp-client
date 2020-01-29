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
        public void Test3()
        {
            X25519Key alice = X25519Key.GenerateKey();
            X25519Key bob = X25519Key.GenerateKey();

            var shared1 = alice.GenerateSharedSecretHex(bob.PublicKey);
            var shared2 = bob.GenerateSharedSecretHex(alice.PublicKey);

            Assert.Equal(shared1, shared2);

            testOutputHelper.WriteLine(shared1);
            testOutputHelper.WriteLine(shared2);
        }
    }
}
