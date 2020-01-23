// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ProtoMessageWrapper.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace OpenVASP.ProtocolMessages.Messages {

  /// <summary>Holder for reflection information generated from ProtoMessageWrapper.proto</summary>
  public static partial class ProtoMessageWrapperReflection {

    #region Descriptor
    /// <summary>File descriptor for ProtoMessageWrapper.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtoMessageWrapperReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChlQcm90b01lc3NhZ2VXcmFwcGVyLnByb3RvEhFwcm90b2J1Zl9vcGVudmFz",
            "cBogUHJvdG9TZXNzaW9uUmVxdWVzdE1lc3NhZ2UucHJvdG8aHlByb3RvU2Vz",
            "c2lvblJlcGx5TWVzc2FnZS5wcm90bxohUHJvdG9UcmFuc2ZlclJlcXVlc3RN",
            "ZXNzYWdlLnByb3RvGh9Qcm90b1RyYW5zZmVyUmVwbHlNZXNzYWdlLnByb3Rv",
            "GiJQcm90b1RyYW5zZmVyRGlzcGF0Y2hNZXNzYWdlLnByb3RvGiZQcm90b1Ry",
            "YW5zZmVyQ29uZmlybWF0aW9uTWVzc2FnZS5wcm90byKwBAoTUHJvdG9NZXNz",
            "YWdlV3JhcHBlchJQChdzZXNzaW9uX3JlcXVlc3RfbWVzc2FnZRgBIAEoCzIt",
            "LnByb3RvYnVmX29wZW52YXNwLlByb3RvU2Vzc2lvblJlcXVlc3RNZXNzYWdl",
            "SAASTAoVc2Vzc2lvbl9yZXBseV9tZXNzYWdlGAIgASgLMisucHJvdG9idWZf",
            "b3BlbnZhc3AuUHJvdG9TZXNzaW9uUmVwbHlNZXNzYWdlSAASUgoYdHJhbnNm",
            "ZXJfcmVxdWVzdF9tZXNzYWdlGAMgASgLMi4ucHJvdG9idWZfb3BlbnZhc3Au",
            "UHJvdG9UcmFuc2ZlclJlcXVlc3RNZXNzYWdlSAASTgoWdHJhbnNmZXJfcmVw",
            "bHlfbWVzc2FnZRgEIAEoCzIsLnByb3RvYnVmX29wZW52YXNwLlByb3RvVHJh",
            "bnNmZXJSZXBseU1lc3NhZ2VIABJUChl0cmFuc2Zlcl9kaXNwYXRjaF9tZXNz",
            "YWdlGAUgASgLMi8ucHJvdG9idWZfb3BlbnZhc3AuUHJvdG9UcmFuc2ZlckRp",
            "c3BhdGNoTWVzc2FnZUgAEl0KHnRyYW5zYWZlcl9jb25maXJtYXRpb25fbWVz",
            "c2FnZRgGIAEoCzIzLnByb3RvYnVmX29wZW52YXNwLlByb3RvVHJhbnNmZXJD",
            "b25maXJtYXRpb25NZXNzYWdlSAASGQoOcmVzZXJ2ZWRfZmllbGQY5wcgASgF",
            "SABCBQoDbXNnQiWqAiJPcGVuVkFTUC5Qcm90b2NvbE1lc3NhZ2VzLk1lc3Nh",
            "Z2VzYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessageReflection.Descriptor, global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessageReflection.Descriptor, global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessageReflection.Descriptor, global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessageReflection.Descriptor, global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessageReflection.Descriptor, global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessageReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::OpenVASP.ProtocolMessages.Messages.ProtoMessageWrapper), global::OpenVASP.ProtocolMessages.Messages.ProtoMessageWrapper.Parser, new[]{ "SessionRequestMessage", "SessionReplyMessage", "TransferRequestMessage", "TransferReplyMessage", "TransferDispatchMessage", "TransaferConfirmationMessage", "ReservedField" }, new[]{ "Msg" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ProtoMessageWrapper : pb::IMessage<ProtoMessageWrapper> {
    private static readonly pb::MessageParser<ProtoMessageWrapper> _parser = new pb::MessageParser<ProtoMessageWrapper>(() => new ProtoMessageWrapper());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ProtoMessageWrapper> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::OpenVASP.ProtocolMessages.Messages.ProtoMessageWrapperReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoMessageWrapper() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoMessageWrapper(ProtoMessageWrapper other) : this() {
      switch (other.MsgCase) {
        case MsgOneofCase.SessionRequestMessage:
          SessionRequestMessage = other.SessionRequestMessage.Clone();
          break;
        case MsgOneofCase.SessionReplyMessage:
          SessionReplyMessage = other.SessionReplyMessage.Clone();
          break;
        case MsgOneofCase.TransferRequestMessage:
          TransferRequestMessage = other.TransferRequestMessage.Clone();
          break;
        case MsgOneofCase.TransferReplyMessage:
          TransferReplyMessage = other.TransferReplyMessage.Clone();
          break;
        case MsgOneofCase.TransferDispatchMessage:
          TransferDispatchMessage = other.TransferDispatchMessage.Clone();
          break;
        case MsgOneofCase.TransaferConfirmationMessage:
          TransaferConfirmationMessage = other.TransaferConfirmationMessage.Clone();
          break;
        case MsgOneofCase.ReservedField:
          ReservedField = other.ReservedField;
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoMessageWrapper Clone() {
      return new ProtoMessageWrapper(this);
    }

    /// <summary>Field number for the "session_request_message" field.</summary>
    public const int SessionRequestMessageFieldNumber = 1;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessage SessionRequestMessage {
      get { return msgCase_ == MsgOneofCase.SessionRequestMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.SessionRequestMessage;
      }
    }

    /// <summary>Field number for the "session_reply_message" field.</summary>
    public const int SessionReplyMessageFieldNumber = 2;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessage SessionReplyMessage {
      get { return msgCase_ == MsgOneofCase.SessionReplyMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.SessionReplyMessage;
      }
    }

    /// <summary>Field number for the "transfer_request_message" field.</summary>
    public const int TransferRequestMessageFieldNumber = 3;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessage TransferRequestMessage {
      get { return msgCase_ == MsgOneofCase.TransferRequestMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.TransferRequestMessage;
      }
    }

    /// <summary>Field number for the "transfer_reply_message" field.</summary>
    public const int TransferReplyMessageFieldNumber = 4;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessage TransferReplyMessage {
      get { return msgCase_ == MsgOneofCase.TransferReplyMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.TransferReplyMessage;
      }
    }

    /// <summary>Field number for the "transfer_dispatch_message" field.</summary>
    public const int TransferDispatchMessageFieldNumber = 5;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessage TransferDispatchMessage {
      get { return msgCase_ == MsgOneofCase.TransferDispatchMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.TransferDispatchMessage;
      }
    }

    /// <summary>Field number for the "transafer_confirmation_message" field.</summary>
    public const int TransaferConfirmationMessageFieldNumber = 6;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessage TransaferConfirmationMessage {
      get { return msgCase_ == MsgOneofCase.TransaferConfirmationMessage ? (global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessage) msg_ : null; }
      set {
        msg_ = value;
        msgCase_ = value == null ? MsgOneofCase.None : MsgOneofCase.TransaferConfirmationMessage;
      }
    }

    /// <summary>Field number for the "reserved_field" field.</summary>
    public const int ReservedFieldFieldNumber = 999;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int ReservedField {
      get { return msgCase_ == MsgOneofCase.ReservedField ? (int) msg_ : 0; }
      set {
        msg_ = value;
        msgCase_ = MsgOneofCase.ReservedField;
      }
    }

    private object msg_;
    /// <summary>Enum of possible cases for the "msg" oneof.</summary>
    public enum MsgOneofCase {
      None = 0,
      SessionRequestMessage = 1,
      SessionReplyMessage = 2,
      TransferRequestMessage = 3,
      TransferReplyMessage = 4,
      TransferDispatchMessage = 5,
      TransaferConfirmationMessage = 6,
      ReservedField = 999,
    }
    private MsgOneofCase msgCase_ = MsgOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public MsgOneofCase MsgCase {
      get { return msgCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void ClearMsg() {
      msgCase_ = MsgOneofCase.None;
      msg_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ProtoMessageWrapper);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ProtoMessageWrapper other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(SessionRequestMessage, other.SessionRequestMessage)) return false;
      if (!object.Equals(SessionReplyMessage, other.SessionReplyMessage)) return false;
      if (!object.Equals(TransferRequestMessage, other.TransferRequestMessage)) return false;
      if (!object.Equals(TransferReplyMessage, other.TransferReplyMessage)) return false;
      if (!object.Equals(TransferDispatchMessage, other.TransferDispatchMessage)) return false;
      if (!object.Equals(TransaferConfirmationMessage, other.TransaferConfirmationMessage)) return false;
      if (ReservedField != other.ReservedField) return false;
      if (MsgCase != other.MsgCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (msgCase_ == MsgOneofCase.SessionRequestMessage) hash ^= SessionRequestMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.SessionReplyMessage) hash ^= SessionReplyMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.TransferRequestMessage) hash ^= TransferRequestMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.TransferReplyMessage) hash ^= TransferReplyMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.TransferDispatchMessage) hash ^= TransferDispatchMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.TransaferConfirmationMessage) hash ^= TransaferConfirmationMessage.GetHashCode();
      if (msgCase_ == MsgOneofCase.ReservedField) hash ^= ReservedField.GetHashCode();
      hash ^= (int) msgCase_;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (msgCase_ == MsgOneofCase.SessionRequestMessage) {
        output.WriteRawTag(10);
        output.WriteMessage(SessionRequestMessage);
      }
      if (msgCase_ == MsgOneofCase.SessionReplyMessage) {
        output.WriteRawTag(18);
        output.WriteMessage(SessionReplyMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferRequestMessage) {
        output.WriteRawTag(26);
        output.WriteMessage(TransferRequestMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferReplyMessage) {
        output.WriteRawTag(34);
        output.WriteMessage(TransferReplyMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferDispatchMessage) {
        output.WriteRawTag(42);
        output.WriteMessage(TransferDispatchMessage);
      }
      if (msgCase_ == MsgOneofCase.TransaferConfirmationMessage) {
        output.WriteRawTag(50);
        output.WriteMessage(TransaferConfirmationMessage);
      }
      if (msgCase_ == MsgOneofCase.ReservedField) {
        output.WriteRawTag(184, 62);
        output.WriteInt32(ReservedField);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (msgCase_ == MsgOneofCase.SessionRequestMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(SessionRequestMessage);
      }
      if (msgCase_ == MsgOneofCase.SessionReplyMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(SessionReplyMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferRequestMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TransferRequestMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferReplyMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TransferReplyMessage);
      }
      if (msgCase_ == MsgOneofCase.TransferDispatchMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TransferDispatchMessage);
      }
      if (msgCase_ == MsgOneofCase.TransaferConfirmationMessage) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TransaferConfirmationMessage);
      }
      if (msgCase_ == MsgOneofCase.ReservedField) {
        size += 2 + pb::CodedOutputStream.ComputeInt32Size(ReservedField);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ProtoMessageWrapper other) {
      if (other == null) {
        return;
      }
      switch (other.MsgCase) {
        case MsgOneofCase.SessionRequestMessage:
          if (SessionRequestMessage == null) {
            SessionRequestMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessage();
          }
          SessionRequestMessage.MergeFrom(other.SessionRequestMessage);
          break;
        case MsgOneofCase.SessionReplyMessage:
          if (SessionReplyMessage == null) {
            SessionReplyMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessage();
          }
          SessionReplyMessage.MergeFrom(other.SessionReplyMessage);
          break;
        case MsgOneofCase.TransferRequestMessage:
          if (TransferRequestMessage == null) {
            TransferRequestMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessage();
          }
          TransferRequestMessage.MergeFrom(other.TransferRequestMessage);
          break;
        case MsgOneofCase.TransferReplyMessage:
          if (TransferReplyMessage == null) {
            TransferReplyMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessage();
          }
          TransferReplyMessage.MergeFrom(other.TransferReplyMessage);
          break;
        case MsgOneofCase.TransferDispatchMessage:
          if (TransferDispatchMessage == null) {
            TransferDispatchMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessage();
          }
          TransferDispatchMessage.MergeFrom(other.TransferDispatchMessage);
          break;
        case MsgOneofCase.TransaferConfirmationMessage:
          if (TransaferConfirmationMessage == null) {
            TransaferConfirmationMessage = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessage();
          }
          TransaferConfirmationMessage.MergeFrom(other.TransaferConfirmationMessage);
          break;
        case MsgOneofCase.ReservedField:
          ReservedField = other.ReservedField;
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoSessionRequestMessage();
            if (msgCase_ == MsgOneofCase.SessionRequestMessage) {
              subBuilder.MergeFrom(SessionRequestMessage);
            }
            input.ReadMessage(subBuilder);
            SessionRequestMessage = subBuilder;
            break;
          }
          case 18: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoSessionReplyMessage();
            if (msgCase_ == MsgOneofCase.SessionReplyMessage) {
              subBuilder.MergeFrom(SessionReplyMessage);
            }
            input.ReadMessage(subBuilder);
            SessionReplyMessage = subBuilder;
            break;
          }
          case 26: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferRequestMessage();
            if (msgCase_ == MsgOneofCase.TransferRequestMessage) {
              subBuilder.MergeFrom(TransferRequestMessage);
            }
            input.ReadMessage(subBuilder);
            TransferRequestMessage = subBuilder;
            break;
          }
          case 34: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferReplyMessage();
            if (msgCase_ == MsgOneofCase.TransferReplyMessage) {
              subBuilder.MergeFrom(TransferReplyMessage);
            }
            input.ReadMessage(subBuilder);
            TransferReplyMessage = subBuilder;
            break;
          }
          case 42: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferDispatchMessage();
            if (msgCase_ == MsgOneofCase.TransferDispatchMessage) {
              subBuilder.MergeFrom(TransferDispatchMessage);
            }
            input.ReadMessage(subBuilder);
            TransferDispatchMessage = subBuilder;
            break;
          }
          case 50: {
            global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessage subBuilder = new global::OpenVASP.ProtocolMessages.Messages.ProtoTransferConfirmationMessage();
            if (msgCase_ == MsgOneofCase.TransaferConfirmationMessage) {
              subBuilder.MergeFrom(TransaferConfirmationMessage);
            }
            input.ReadMessage(subBuilder);
            TransaferConfirmationMessage = subBuilder;
            break;
          }
          case 7992: {
            ReservedField = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
