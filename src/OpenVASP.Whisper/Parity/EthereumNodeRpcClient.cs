using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenVASP.Whisper.Parity
{
    public class EthereumNodeRpcClient
    {
        private readonly Uri parityUrl;
        private readonly HttpClient httpClient;

        public EthereumNodeRpcClient(string parityUrl, HttpClient httpClient)
        {
            this.parityUrl = new Uri(parityUrl);
            this.httpClient = httpClient;
        }

        public async Task<T> ExecuteAsync<T>(Request request)
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var byteArray = Encoding.ASCII.GetBytes(serialized);

            using (var stream = new MemoryStream(byteArray))
            using (var content = new StreamContent(stream))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await this.httpClient.PostAsync(this.parityUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString);
            }
        }

        [DataContract]
        public class Request
        {
            [DataMember(Name = "jsonrpc")]
            public static string JsonRpc => "2.0";

            [DataMember(Name = "id")]
            public string Id { get; set; }

            [DataMember(Name = "method")]
            public string Method { get; set; }

            [DataMember(Name = "params")]
            public List<object> Params { get; set; }
        }

        [DataContract]
        public class Response<T>
        {
            [DataMember(Name = "result")]
            public T Result { get; set; }
        }

        [DataContract]
        public class RpcError
        {
            private RpcError()
            {
            }
            /// <summary>
            /// Rpc error code
            /// </summary>
            [DataMember(Name = "code")]
            public int Code { get; private set; }

            /// <summary>
            /// Error message (Required)
            /// </summary>
            [DataMember(Name = "message")]
            public string Message { get; private set; }
        }

        [DataContract]
        public class WhisperMessage
        {
            [DataMember(Name = "from")]
            public string From { get; set; }

            [DataMember(Name = "recipient")]
            public string Recipient { get; set; }

            [DataMember(Name = "hash")]
            public string Hash { get; set; }

            [DataMember(Name = "Padding")]
            public string padding { get; set; }

            [DataMember(Name = "payload")]
            public string Payload { get; set; }

            [DataMember(Name = "pow")]
            public string Pow { get; set; }

            [DataMember(Name = "recipientPublicKey")]
            public string RecipientPublicKey { get; set; }

            [DataMember(Name = "sig")]
            public string Signature { get; set; }

            [DataMember(Name = "timestamp")]
            public TimeSpan Timestamp { get; set; }

            [DataMember(Name = "topic")]
            public IEnumerable<string> Topics { get; set; }

            [DataMember(Name = "ttl")]
            public string Ttl { get; set; }
        }

        [DataContract]
        public class ShhPost
        {
            //    [DataMember(Name = "public")] 
            //    public string Public { get; set; }

            [DataMember(Name = "ttl")]
            public int Ttl { get; set; }

            [DataMember(Name = "topics")]
            public IEnumerable<string> Topics { get; set; }

            [DataMember(Name = "priority")]
            public int Priority { get; set; }

            [DataMember(Name = "payload")]
            public string Payload { get; set; }

            //[DataMember(Name = "from")]
            //public string From { get; set; }
        }
        
    }
}