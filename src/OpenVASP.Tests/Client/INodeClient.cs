namespace OpenVASP.Tests.Client
{
    public interface INodeClient
    {
        IEthereumRpc EthereumRpc { get; }
        IWhisperRpc WhisperRpc { get; set; }
    }
}