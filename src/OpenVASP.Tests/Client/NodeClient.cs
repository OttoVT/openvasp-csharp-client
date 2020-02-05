namespace OpenVASP.Tests.Client
{
    public class NodeClient : INodeClient
    {
        public IEthereumRpc EthereumRpc { get; set; }
        public IWhisperRpc WhisperRpc { get; set; }
        public ITransportClient TransportClient { get; set; }
    }
}