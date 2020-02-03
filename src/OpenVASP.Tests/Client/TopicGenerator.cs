using System;
using OpenVASP.Messaging;

namespace OpenVASP.Tests.Client
{
    public static class TopicGenerator
    {
        private static Random _random = new Random();

        public static string GenerateSessionTopic()
        {
            var bytes = new byte[4];
            _random.NextBytes(bytes);

            return bytes.CustomToHex(true);
        }
    }
}