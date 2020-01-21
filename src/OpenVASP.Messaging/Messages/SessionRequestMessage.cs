namespace OpenVASP.Messaging.Messages
{
    public class SessionRequestMessage : MessageBase
    {
        public SessionRequestMessage(Message message, HandShake handshake, VaspInformation vasp)
        {
            Message = message;
            HandShake = handshake;
            VASP = vasp;
        }

        public HandShake HandShake { get; private set; }

        public Message Message { get; private set; }

        public VaspInformation VASP { get; private set; }
    }
}
