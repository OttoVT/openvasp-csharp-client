using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Nethereum.Hex.HexConvertors.Extensions;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.ProtocolMessages.Messages;

namespace OpenVASP.Whisper
{
    public class WhisperMessageFormatter : IMessageFormatter
    {
        public MessageEnvelope GetEnvelope(MessageBase messageBase)
        {
            var messageEnvelope = new MessageEnvelope()
            {
                EncryptionType = messageBase.MessageEnvelope.EncryptionType,
                Topic = messageBase.MessageEnvelope.Topic,
                Signature = messageBase.MessageEnvelope.Signature,
                EncryptionKey = messageBase.MessageEnvelope.EncryptionKey
            };

            return messageEnvelope;
        }

        public string GetPayload(SessionRequestMessage sessionRequestMessage)
        {
            var proto = Mapper.MapSessionRequestMessageToProto(sessionRequestMessage);
            var wrapper = new ProtoMessageWrapper()
            {
                SessionRequestMessage = proto
            };

            var payload = wrapper.ToByteArray().ToHex(prefix: true);

            return payload;
        }

        //public MessageType GetMessageType(string payload)
        //{
        //    var bytes = payload.HexToByteArray();
        //    var protoMessage = Any.Parser.ParseFrom(bytes);
        //    var type = (MessageType) protoMessage.MessageType;

        //    return type;
        //}

        public MessageBase Deserialize(string payload, MessageEnvelope messageEnvelope)
        {
            var bytes = payload.HexToByteArray();
            var wrapper = ProtoMessageWrapper.Parser.ParseFrom(bytes);
            MessageBase message = null;
            switch (wrapper.MsgCase)
            {
                case ProtoMessageWrapper.MsgOneofCase.SessionRequestMessage:
                {
                    message = Mapper.MapSessionRequestMessageFromProto(wrapper.SessionRequestMessage);

                    break;
                }

                default:
                    break;
            }

            if (message != null)
            {
                message.MessageEnvelope = messageEnvelope;
            }

            return message;
        }
    }
}