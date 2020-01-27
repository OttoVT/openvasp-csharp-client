using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Messaging.Messages
{
    public class SessionRequestMessage : MessageBase
    {
        public SessionRequestMessage(string sessionId, HandShakeRequest handshake, VaspInformation vasp, string messageId) 
            : base(MessageType.SessionRequest, vasp)
        {
            MessageType = MessageType.SessionRequest;
            Message = new Message(messageId, sessionId, "1");
            HandShake = handshake;
        }

        public HandShakeRequest HandShake { get; private set; }

        public Message Message { get; private set; }
    }
}
