using System;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public class VaspSession
    {
        public VaspSession()
        {

        }

        public string SessionId { get; set; }

        public bool IsOriginator { get; set; }

        public bool IsBeneficiary { get; set; }

        public VaspSessionCounterparty Counterparty { get; set; }


        public async Task<TransferReply> TransferRequestAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TransferConfirmationMessage> TransferDispatchAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Termination()
        {
            throw new NotImplementedException();
        }
    }
}