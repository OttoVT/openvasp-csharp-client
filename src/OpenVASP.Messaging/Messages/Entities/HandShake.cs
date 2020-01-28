namespace OpenVASP.Messaging.Messages.Entities
{
    public class HandShakeRequest
    {
        public string TopicA { get; private set; }

        public string AesGsmSharedKey { get; private set; }

        public HandShakeRequest(string topicA, string ecdhPubKey)
        {
            TopicA = topicA;
            AesGsmSharedKey = ecdhPubKey;
        }
    }

    public class HandShakeResponse
    {
        public string TopicB { get; private set; }

        public HandShakeResponse(string topicB)
        {
            TopicB = topicB;
        }
    }
}