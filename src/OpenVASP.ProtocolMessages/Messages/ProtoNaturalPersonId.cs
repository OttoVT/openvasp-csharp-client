// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ProtoNaturalPersonId.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace OpenVASP.ProtocolMessages.Messages {

  /// <summary>Holder for reflection information generated from ProtoNaturalPersonId.proto</summary>
  public static partial class ProtoNaturalPersonIdReflection {

    #region Descriptor
    /// <summary>File descriptor for ProtoNaturalPersonId.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ProtoNaturalPersonIdReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChpQcm90b05hdHVyYWxQZXJzb25JZC5wcm90bxIRcHJvdG9idWZfb3BlbnZh",
            "c3AieQoUUHJvdG9OYXR1cmFsUGVyc29uSWQSGwoTaWRlbnRpZmljYXRpb25f",
            "dHlwZRgBIAEoBRISCgppZGVudGlmaWVyGAIgASgJEhcKD2lzc3VpbmdfY291",
            "bnRyeRgDIAEoCRIXCg9ub25zdGF0ZV9pc3N1ZXIYBCABKAlCJaoCIk9wZW5W",
            "QVNQLlByb3RvY29sTWVzc2FnZXMuTWVzc2FnZXNiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::OpenVASP.ProtocolMessages.Messages.ProtoNaturalPersonId), global::OpenVASP.ProtocolMessages.Messages.ProtoNaturalPersonId.Parser, new[]{ "IdentificationType", "Identifier", "IssuingCountry", "NonstateIssuer" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class ProtoNaturalPersonId : pb::IMessage<ProtoNaturalPersonId> {
    private static readonly pb::MessageParser<ProtoNaturalPersonId> _parser = new pb::MessageParser<ProtoNaturalPersonId>(() => new ProtoNaturalPersonId());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ProtoNaturalPersonId> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::OpenVASP.ProtocolMessages.Messages.ProtoNaturalPersonIdReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoNaturalPersonId() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoNaturalPersonId(ProtoNaturalPersonId other) : this() {
      identificationType_ = other.identificationType_;
      identifier_ = other.identifier_;
      issuingCountry_ = other.issuingCountry_;
      nonstateIssuer_ = other.nonstateIssuer_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ProtoNaturalPersonId Clone() {
      return new ProtoNaturalPersonId(this);
    }

    /// <summary>Field number for the "identification_type" field.</summary>
    public const int IdentificationTypeFieldNumber = 1;
    private int identificationType_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int IdentificationType {
      get { return identificationType_; }
      set {
        identificationType_ = value;
      }
    }

    /// <summary>Field number for the "identifier" field.</summary>
    public const int IdentifierFieldNumber = 2;
    private string identifier_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Identifier {
      get { return identifier_; }
      set {
        identifier_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "issuing_country" field.</summary>
    public const int IssuingCountryFieldNumber = 3;
    private string issuingCountry_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string IssuingCountry {
      get { return issuingCountry_; }
      set {
        issuingCountry_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "nonstate_issuer" field.</summary>
    public const int NonstateIssuerFieldNumber = 4;
    private string nonstateIssuer_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string NonstateIssuer {
      get { return nonstateIssuer_; }
      set {
        nonstateIssuer_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ProtoNaturalPersonId);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ProtoNaturalPersonId other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (IdentificationType != other.IdentificationType) return false;
      if (Identifier != other.Identifier) return false;
      if (IssuingCountry != other.IssuingCountry) return false;
      if (NonstateIssuer != other.NonstateIssuer) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (IdentificationType != 0) hash ^= IdentificationType.GetHashCode();
      if (Identifier.Length != 0) hash ^= Identifier.GetHashCode();
      if (IssuingCountry.Length != 0) hash ^= IssuingCountry.GetHashCode();
      if (NonstateIssuer.Length != 0) hash ^= NonstateIssuer.GetHashCode();
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
      if (IdentificationType != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(IdentificationType);
      }
      if (Identifier.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Identifier);
      }
      if (IssuingCountry.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(IssuingCountry);
      }
      if (NonstateIssuer.Length != 0) {
        output.WriteRawTag(34);
        output.WriteString(NonstateIssuer);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (IdentificationType != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(IdentificationType);
      }
      if (Identifier.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Identifier);
      }
      if (IssuingCountry.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(IssuingCountry);
      }
      if (NonstateIssuer.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(NonstateIssuer);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ProtoNaturalPersonId other) {
      if (other == null) {
        return;
      }
      if (other.IdentificationType != 0) {
        IdentificationType = other.IdentificationType;
      }
      if (other.Identifier.Length != 0) {
        Identifier = other.Identifier;
      }
      if (other.IssuingCountry.Length != 0) {
        IssuingCountry = other.IssuingCountry;
      }
      if (other.NonstateIssuer.Length != 0) {
        NonstateIssuer = other.NonstateIssuer;
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
          case 8: {
            IdentificationType = input.ReadInt32();
            break;
          }
          case 18: {
            Identifier = input.ReadString();
            break;
          }
          case 26: {
            IssuingCountry = input.ReadString();
            break;
          }
          case 34: {
            NonstateIssuer = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
