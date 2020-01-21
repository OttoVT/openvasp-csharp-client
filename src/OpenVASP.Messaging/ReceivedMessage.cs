using System;

namespace OpenVASP.Messaging
{
    public class ReceivedMessage
    {
        public MessageEnvelope MessageEnvelope { get; set; }

        public string Payload { get; set; }
    }

    public class ReceivedMessage<T> where T: class
    {
        public ReceivedMessage(object obj)
        {
            if (obj.GetType() != typeof(T))
            {
                throw new ArgumentException("Wrong type", nameof(obj));
            }

            Message = (T) obj;
        }
        public T Message { get; private set; }
    }
}
