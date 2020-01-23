namespace OpenVASP.Messaging.Messages
{
    public class HandShakeRequest
    {
        public string TopicA { get; private set; }

        public string ECDHPublicKey { get; private set; }

        public HandShakeRequest(string topicA, string ecdhPubKey)
        {
            TopicA = topicA;
            ECDHPublicKey = ecdhPubKey;
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