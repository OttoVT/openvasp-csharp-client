using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Messaging.Messages
{
    public abstract class MessageBase
    {
        public MessageBase(MessageType messageType, VaspInformation vaspInformation)
        {
            MessageType = messageType;
            VASP = vaspInformation;
        }

        public MessageType MessageType { get; }

        public string Comment { get; set; } = string.Empty;

        public VaspInformation VASP { get; }
    }
}
