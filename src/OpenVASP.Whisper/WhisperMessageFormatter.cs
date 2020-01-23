using System;
using Google.Protobuf;
using Nethereum.Hex.HexConvertors.Extensions;
using OpenVASP.Messaging;
using OpenVASP.Messaging.Messages;
using OpenVASP.ProtocolMessages.Messages;
using OpenVASP.Whisper.Mappers;

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

        public string GetPayload(MessageBase messageBase)
        {
            var wrapper = new ProtoMessageWrapper();

            switch (messageBase.MessageType)
            {
                case MessageType.SessionRequest:
                    {
                        var proto = SessionRequestMessageMapper.MapToProto((SessionRequestMessage)messageBase);
                        wrapper.SessionRequestMessage = proto;

                        break;
                    }

                case MessageType.SessionReply:
                    {
                        var proto = SessionReplyMessageMapper.MapToProto((SessionReplyMessage)messageBase);
                        wrapper.SessionReplyMessage = proto;

                        break;
                    }

                case MessageType.TransferRequest:
                {
                    var proto = TransferRequestMessageMapper.MapToProto((TransferRequestMessage)messageBase);
                    wrapper.TransferRequestMessage = proto;

                    break;
                }

                case MessageType.TransferReply:
                {
                    var proto = TransferReplyMessageMapper.MapToProto((TransferReplyMessage)messageBase);
                    wrapper.TransferReplyMessage = proto;

                    break;
                }

                default:
                    throw new ArgumentException($"Message of type {messageBase.GetType()} contains enum message type {messageBase.MessageType}" +
                                                $"which is not supported");
            }

            var payload = wrapper.ToByteArray().ToHex(prefix: true);

            return payload;
        }

        public MessageBase Deserialize(string payload, MessageEnvelope messageEnvelope)
        {
            var bytes = payload.HexToByteArray();
            var wrapper = ProtoMessageWrapper.Parser.ParseFrom(bytes);
            MessageBase message = null;
            switch (wrapper.MsgCase)
            {
                case ProtoMessageWrapper.MsgOneofCase.SessionRequestMessage:
                    {
                        message = SessionRequestMessageMapper.MapFromProto(wrapper.SessionRequestMessage);

                        break;
                    }

                case ProtoMessageWrapper.MsgOneofCase.SessionReplyMessage:
                    {
                        message = SessionRequestMessageMapper.MapFromProto(wrapper.SessionRequestMessage);

                        break;
                    }

                case ProtoMessageWrapper.MsgOneofCase.TransferRequestMessage:
                {
                    message = TransferRequestMessageMapper.MapFromProto(wrapper.TransferRequestMessage);

                    break;
                }

                case ProtoMessageWrapper.MsgOneofCase.TransferReplyMessage:
                {
                    message = TransferReplyMessageMapper.MapFromProto(wrapper.TransferReplyMessage);

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