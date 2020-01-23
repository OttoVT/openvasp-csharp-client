﻿namespace OpenVASP.Messaging.Messages
{
    public class SessionRequestMessage : MessageBase
    {
        public SessionRequestMessage(Message message, HandShakeRequest handshake, VaspInformation vasp)
        {
            MessageType = MessageType.SessionRequest;
            Message = message;
            HandShake = handshake;
            VASP = vasp;
        }

        public HandShakeRequest HandShake { get; private set; }

        public Message Message { get; private set; }

        public VaspInformation VASP { get; private set; }
    }
}