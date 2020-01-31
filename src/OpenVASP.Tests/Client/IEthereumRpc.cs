using System.Threading.Tasks;
using OpenVASP.CSharpClient;

namespace OpenVASP.Tests.Client
{
    public interface IEthereumRpc
    {
        Task<VaspContractInfo> GetVaspContractInfoAync(string vaspSmartContractInfo);
    }
}