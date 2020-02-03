using System;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Messaging.Messages
{
    public class TerminationMessage : MessageBase
    {
        public TerminationMessage(Message message, VaspInformation vasp)
        {
            MessageType = MessageType.Termination;
            Message = message;
            VASP = vasp;
        }

        public TerminationMessage(string sessionId, TerminationMessageCode messageCode, VaspInformation vasp)
        {
            Message = new Message(
                Guid.NewGuid().ToString(),
                sessionId,
                GetMessageCode(messageCode));
            VASP = vasp;
        }

        public Message Message { get; private set; }

        public VaspInformation VASP { get; private set; }

        public static string GetMessageCode(TerminationMessageCode messageCode)
        {
            return messageCode.ToString();
        }
        public enum TerminationMessageCode
        {
            SessionClosedTransferOccured = 1,
            SessionClosedTransferDeclinedByBeneficiaryVasp = 2,
            SessionClosedTransferCancelledByOriginator = 3,
        }
    }
}
