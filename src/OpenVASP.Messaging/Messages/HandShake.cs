namespace OpenVASP.Messaging.Messages
{
    public class HandShake
    {
        public string TopicA { get; private set; }

        public string TopicB { get; private set; }

        public string ECDHPublicKey { get; private set; }

        public HandShake(string topicA, string topicB, string ecdhPubKey)
        {
            TopicA = topicA;
            TopicB = topicB;
            ECDHPublicKey = ecdhPubKey;
        }
    }
}