namespace OpenVASP.Tests.Client
{
    public class SessionTerminationEvent
    {
        public SessionTerminationEvent(string sessionId)
        {
            this.SessionId = sessionId;
        }
        public string SessionId { get;}
    }
}