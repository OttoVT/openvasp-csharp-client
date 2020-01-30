using System;
using System.Threading.Tasks;
using OpenVASP.CSharpClient.Cryptography;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.CSharpClient
{
    public class Session
    {
        private readonly string _topicA;
        private readonly string _sessionId;
        private readonly X25519Key _key;
        private readonly VaspContractInfo _beneficiarVaspContractInfo;
        private readonly VaspInformation _originatorVaspInformation;
        private readonly string _sharedKey;

        public Session(
            string topicA,
            string sessionId, 
            X25519Key key,
            VaspContractInfo beneficiarVaspContractInfo, 
            VaspInformation originatorVaspInformation)
        {
            this._topicA = topicA;
            this._sessionId = sessionId;
            this._key = key;
            this._beneficiarVaspContractInfo = beneficiarVaspContractInfo;
            this._originatorVaspInformation = originatorVaspInformation;
            this._sharedKey = _key.GenerateSharedSecretHex(_beneficiarVaspContractInfo.HandshakeKey);
        }

        public async Task StartAsync()
        {
            await SetupObservationAsync(topic: _topicA, sharedKey: _sharedKey, beneficiarySigningKey: _beneficiarVaspContractInfo.SigningKey);
            var sessionRequestMessage = new SessionRequestMessage(
                _sessionId, 
                new HandShakeRequest(_topicA, _key.PublicKey), 
                _originatorVaspInformation
                );

            await SendSessionRequestAsync(sessionRequestMessage);
        }

        private async Task SendSessionRequestAsync(SessionRequestMessage sessionRequestMessage)
        {
            throw new NotImplementedException();
        }

        private async Task SetupObservationAsync(string topic, string sharedKey, string beneficiarySigningKey)
        {
            throw new NotImplementedException();
        }
    }

    public class BeneficiaryVaspListener
    {
        private Task _worker;
        private VaspInformation _beneficiaryVaspInformation;
        private X25519Key _handshakeKey;
        private string _signingKey;

        public BeneficiaryVaspListener(VaspInformation beneficiaryVaspInformation, X25519Key handshakeKey, string signingKey)
        {
            this._beneficiaryVaspInformation = beneficiaryVaspInformation;
            this._handshakeKey = handshakeKey;
            this._signingKey = signingKey;
        }

        public void Start()
        {
            _worker = Task.Run(() =>
            {

            });
        }
    }

    public class SessionFactory
    {
        public SessionFactory()
        {

        }

        public async Task<Session> CreateSessionAsync(TransferInstruction transferInstruction, VaspInformation originatorVaspInformation)
        {
            var vaspCode = transferInstruction.Vaan.VaspCode;
            var beneficiarVaspContractInfo = await AuthenticateVaspIdentity(vaspCode);

            await AuthorizeBeneficiaryVasp(beneficiarVaspContractInfo);
            await ValidateTransferInstruction(transferInstruction);
            (string topicA, string sessionId, X25519Key key) = await GenerateHeadersAndKey();
            var session = new Session(topicA, sessionId, key, beneficiarVaspContractInfo, originatorVaspInformation);

            return session;
        }

        private async Task<(string, string, X25519Key)> GenerateHeadersAndKey()
        {
            throw new NotImplementedException();
        }

        private async Task ValidateTransferInstruction(TransferInstruction transferInstruction)
        {
            throw new NotImplementedException();
        }

        private async Task AuthorizeBeneficiaryVasp(VaspContractInfo beneficiarVaspContractInfo)
        {
            throw new NotImplementedException();
        }

        private async Task<VaspContractInfo> AuthenticateVaspIdentity(VaspCode vaspCode)
        {
            //Go To ENS to receive vasp smart contract address; 
            var vaspIdentityAddress = GetVaspIdentity(vaspCode.Code);
            var vaspContractInfo = GetVaspContractInfo(vaspIdentityAddress);

            return vaspContractInfo;
        }

        private VaspContractInfo GetVaspContractInfo(string vaspIdentityAddress)
        {
            throw new NotImplementedException();
        }

        private string GetVaspIdentity(string vaspCodeCode)
        {
            throw new NotImplementedException();
        }
    }
}
