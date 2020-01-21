using System;

namespace OpenVASP.Messaging.Messages
{
    public class Transaction
    {
        public string TransactionId { get; set; }

        public DateTime DateTime { get; set; }

        public string SendingAddress { get; set; }
    }
}