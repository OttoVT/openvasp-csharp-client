using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nethereum.Signer;
using OpenVASP.Whisper;
using OpenVASP.Whisper.Parity;
using Xunit;

namespace OpenVASP.Tests
{
    public class ParityWhisperFlowTest
    {
        //This test proves that whisper does not work correctly for parity
        [Fact(Skip = "Disabled")]
        public async Task WhisperFlowTestAsync()
        {
            var ethereumShhRpcClient = new EthereumShhRpcClient("", new HttpClient());
            var ethKey = EthECKey.GenerateKey();

            try
            {
                var kId = await ethereumShhRpcClient.NewKeyPairAsync();

                var topic = "0xb10e2d527612073b26eecdfd717e6a320cf44b4afac2b0732d9fcbe2b7fa0cf6"; //"0x" + "My Topic".GetHashCode().ToString("x");

                var receiverPubKey = await ethereumShhRpcClient.GetPublicKeyAsync(kId);

                var messageFilter = await ethereumShhRpcClient.NewMessageFilterAsync(kId, new[] { topic });

                await ethereumShhRpcClient.PostAsync(kId, topic);

                for (int i = 0; i < 100; i++)
                {
                    var messages = await ethereumShhRpcClient.GetFilterMessagesAsync(messageFilter);

                    await Task.Delay(10000);
                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
