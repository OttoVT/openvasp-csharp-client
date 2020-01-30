using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.CSharpClient
{
    public class TransferInstruction
    {
        public VirtualAssetssAccountNumber Vaan { get; set; }

        public VirtualAssetTransfer VirtualAssetTransfer { get; set; }
    }
}