using System.Threading.Tasks;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public interface IEnsProvider
    {
        Task<string> GetContractAddressByVaspCodeAsync(VaspCode vaspCode);
    }
}