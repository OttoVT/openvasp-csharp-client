using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;

namespace OpenVASP.Whisper.Parity
{
    public class EthereumShhRpcClient : EthereumNodeRpcClient
    {
        public EthereumShhRpcClient(string parityUrl, HttpClient httpClient) : base(parityUrl, httpClient)
        {
        }

        public async Task<string> NewKeyPairAsync()
        {
            var result = await base.ExecuteAsync<Response<string>>(new Request()
            {
                Id = "1",
                Method = "shh_newKeyPair",
                Params = new List<object>()
                    {}
            });

            return result.Result;
        }

        public async Task<string> GetPublicKeyAsync(string kId)
        {
            var result = await base.ExecuteAsync<Response<string>>(new Request()
            {
                Id = "1",
                Method = "shh_getPublicKey",
                Params = new List<object>()
                {
                    kId
                }
            });

            return result.Result;
        }

        public async Task<string> NewMessageFilterAsync(string privateKeyId, IEnumerable<string> topics)
        {
            var result = await base.ExecuteAsync<Response<string>>(new Request()
            {
                Id = "1",
                Method = "shh_newMessageFilter",
                Params = new List<object>()
                {
                    new
                    {
                        //decryptWith = privateKeyId, 
                        //from = (string)null, 
                        topics = topics.ToArray()
                    }
                }
            });

            return result.Result;
        }

        public async Task<string> PostAsync(string receiverPubKey, string topic)
        {
            var result = await base.ExecuteAsync<Response<string>>(new Request()
            {
                Id = "1",
                Method = "shh_post",
                Params = new List<object>()
                {
                    new ShhPost()
                    {
                        //Public = receiverPubKey,
                        //From = receiverPubKey,
                        Ttl = 500,
                        Topics = new [] {topic},
                        Priority = 25,
                        Payload = "Hello there!".ToHexUTF8()
                    }
                }
            });

            return result.Result;
        }

        public async Task<WhisperMessage[]> GetFilterMessagesAsync(string filter)
        {
            var result = await base.ExecuteAsync<Response<WhisperMessage[]>>(new Request()
            {
                Id = "1",
                Method = "shh_getFilterMessages",
                Params = new List<object>()
                {
                    filter
                }
            });

            return result.Result;
        }
    }
}