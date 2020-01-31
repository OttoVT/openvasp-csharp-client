using System;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;
using OpenVASP.CSharpClient;
using OpenVASP.CSharpClient.Cryptography;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using OpenVASP.Whisper;
using Xunit;
using Xunit.Abstractions;

namespace OpenVASP.Tests.Client
{
    public class ClientExample
    {
        private readonly ITestOutputHelper testOutputHelper;

        public INodeClient NodeClient { get; set; }

        public VaspTestSettings Settings { get; set; }

        public ClientExample(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            NodeClient = new NodeClient()
            {
                EthereumRpc = new EthereumRpc(new Web3()),
                WhisperRpc = new WhisperRpc(new Web3(), new WhisperMessageFormatter())
            };
            Settings = new VaspTestSettings()
            {
                PersonHandshakePrivateKeyHex = "0xc0e45195b43ef54c3a7ce526c3d2e482e4f73b3c5816038f8391ef31e2540d50",
                PersonSignaturePrivateKeyHex = "0x790a3437381e0ca44a71123d56dc64a6209542ddd58e5a56ecdb13134e86f7c6",
                VaspSmartContractAddressPerson = "0x6befaf0656b953b188a0ee3bf3db03d07dface61",
            };
        }

        [Fact]
        public async Task CreateVaspForNaturalPerson_Builder()
        {
            var builder = new VaspInformationBuilder(NodeClient.EthereumRpc);

            (VaspInformation vaspInfo, VaspContractInfo vaspContractInfo) = await builder.CreateForNaturalPersonAsync(
                Settings.VaspSmartContractAddressPerson,
                Settings.NaturalPersonIds,
                Settings.PlaceOfBirth);

            VaspClient vasp = VaspClient.Create(
                vaspInfo,
                vaspContractInfo,
                Settings.PersonHandshakePrivateKeyHex,
                Settings.PersonSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            // VASP paramaters must be derived from smart contract
            Assert.NotNull(vasp.VaspInfo.Name);
            Assert.NotNull(vasp.VaspInfo.VaspIdentity);
            Assert.NotNull(vasp.VaspInfo.PostalAddress);

            // VASP natural person parameters should be same what we pass in constructor
            Assert.True(vasp.VaspInfo.NaturalPersonIds.SequenceEqual(Settings.NaturalPersonIds));
            Assert.Equal(vasp.VaspInfo.PlaceOfBirth, Settings.PlaceOfBirth);

            Assert.Null(vasp.VaspInfo.JuridicalPersonIds);
            Assert.Null(vasp.VaspInfo.BIC);
        }

        [Fact]
        public async Task CreateVaspForNaturalPerson_Static()
        {
            (VaspInformation vaspInfo, VaspContractInfo vaspContractInfo) = await VaspInformationBuilder.CreateForNaturalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressPerson,
                Settings.NaturalPersonIds,
                Settings.PlaceOfBirth);

            VaspClient vasp = VaspClient.Create(
                vaspInfo,
                vaspContractInfo,
                Settings.PersonHandshakePrivateKeyHex,
                Settings.PersonSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            // VASP paramaters must be derived from smart contract
            Assert.NotNull(vasp.VaspInfo.Name);
            Assert.NotNull(vasp.VaspInfo.VaspIdentity);
            Assert.NotNull(vasp.VaspInfo.PostalAddress);

            // VASP natural person parameters should be same what we pass in constructor
            Assert.True(vasp.VaspInfo.NaturalPersonIds.SequenceEqual(Settings.NaturalPersonIds));
            Assert.Equal(vasp.VaspInfo.PlaceOfBirth, Settings.PlaceOfBirth);

            Assert.Null(vasp.VaspInfo.JuridicalPersonIds);
            Assert.Null(vasp.VaspInfo.BIC);
        }

        [Fact]
        public async Task CreateVaspForJuridicalPerso()
        {
            (VaspInformation vaspInfo, VaspContractInfo vaspContractInfo) = await VaspInformationBuilder.CreateForJuridicalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressJuridical,
                Settings.JuridicalIds);

            VaspClient vasp = VaspClient.Create(
                vaspInfo,
                vaspContractInfo,
                Settings.JuridicalHandshakePrivateKeyHex,
                Settings.JuridicalSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            // VASP paramaters must be derived from smart contract
            Assert.NotNull(vasp.VaspInfo.Name);
            Assert.NotNull(vasp.VaspInfo.VaspIdentity);
            Assert.NotNull(vasp.VaspInfo.PostalAddress);

            // VASP natural person parameters should be same what we pass in constructor
            Assert.True(vasp.VaspInfo.JuridicalPersonIds.SequenceEqual(Settings.JuridicalIds));

            Assert.Null(vasp.VaspInfo.NaturalPersonIds);
            Assert.Null(vasp.VaspInfo.PlaceOfBirth);
            Assert.Null(vasp.VaspInfo.BIC);
        }

        [Fact]
        public async Task CreateVaspForBank()
        {
            var signature = new EthECKey(Settings.PersonSignaturePrivateKeyHex);
            var handshake = X25519Key.ImportKey(Settings.PersonHandshakePrivateKeyHex);

            var signPub = signature.GetPubKey().ToHex(prefix: true);
            var handshakePub = handshake.PublicKey;

            (VaspInformation vaspInfo, VaspContractInfo vaspContractInfo) = await VaspInformationBuilder.CreateForBankAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressBank,
                Settings.Bic);

            VaspClient vasp = VaspClient.Create(
                vaspInfo,
                vaspContractInfo,
                Settings.BankHandshakePrivateKeyHex,
                Settings.BankSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            // VASP paramaters must be derived from smart contract
            Assert.NotNull(vasp.VaspInfo.Name);
            Assert.NotNull(vasp.VaspInfo.VaspIdentity);
            Assert.NotNull(vasp.VaspInfo.PostalAddress);

            // VASP natural person parameters should be same what we pass in constructor
            Assert.Equal(vasp.VaspInfo.BIC, Settings.Bic);

            Assert.Null(vasp.VaspInfo.NaturalPersonIds);
            Assert.Null(vasp.VaspInfo.PlaceOfBirth);
            Assert.Null(vasp.VaspInfo.JuridicalPersonIds);
        }

        [Fact]
        public async Task CreateSessionBetweenVASPs()
        {
            (VaspInformation vaspInfoPerson, VaspContractInfo vaspContractInfoPerson) = await VaspInformationBuilder.CreateForNaturalPersonAsync(
                 NodeClient.EthereumRpc,
                 Settings.VaspSmartContractAddressPerson,
                 Settings.NaturalPersonIds,
                 Settings.PlaceOfBirth);

            VaspClient originator = VaspClient.Create(
                vaspInfoPerson,
                vaspContractInfoPerson,
                Settings.PersonHandshakePrivateKeyHex,
                Settings.PersonSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            (VaspInformation vaspInfoJuridical, VaspContractInfo vaspContractInfoJuridical) = await VaspInformationBuilder.CreateForJuridicalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressJuridical,
                Settings.JuridicalIds);

            VaspClient beneficiary = VaspClient.Create(
                vaspInfoJuridical,
                vaspContractInfoJuridical,
                Settings.JuridicalHandshakePrivateKeyHex,
                Settings.JuridicalSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            var beneficiaryVaan = VirtualAssetssAccountNumber.Create(vaspInfoPerson.GetVaspCode(), "524ee3fb082809");

            IVaspMessageHandler messageHandler = new VaspMessageHandlerCallbacks(
                sessionRequest => Task.FromResult((SessionReplyMessage)null),
                request => Task.FromResult((TransferReplyMessage)null),
                dispatch => Task.FromResult((TransferConfirmationMessage)null));

            originator.RunListener(messageHandler);
            beneficiary.RunListener(messageHandler);

            // change enum to string and add constants
            //beneficiary.TransferRequest += request => new TransferReply(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10", "1223");
            //beneficiary.TransferDispatch += message => new TransferConfirmationMessage();

            VaspSession session = await originator.CreateSessionAsync(beneficiaryVaan);

            TransferReply transferReply1 = await session.TransferRequestAsync();
            TransferReply transferReply2 = await session.TransferRequestAsync();

            TransferConfirmationMessage transferConformation1 = await session.TransferDispatchAsync();
            TransferConfirmationMessage transferConformation2 = await session.TransferDispatchAsync();

            Assert.Equal(1, originator.GetActiveSessions().Count);
            Assert.True(originator.GetActiveSessions().First().IsOriginator);

            Assert.Equal(1, beneficiary.GetActiveSessions().Count);
            Assert.True(beneficiary.GetActiveSessions().First().IsBeneficiary);

            await session.Termination();

            originator.Dispose();
            beneficiary.Dispose();

            testOutputHelper.WriteLine("End of test");
        }

    }

    public class NodeClient : INodeClient
    {
        public IEthereumRpc EthereumRpc { get; set; }
        public IWhisperRpc WhisperRpc { get; set; }
    }

    public interface IVaspMessageHandler
    {
        Task<SessionReplyMessage> SessionRequestHandlerAsync(SessionRequestMessage request);

        Task<TransferReplyMessage> TransferRequestHandlerAsync(TransferRequestMessage request);

        Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request);
    }

    public class VaspMessageHandlerCallbacks : IVaspMessageHandler
    {
        private readonly Func<TransferRequestMessage, Task<TransferReplyMessage>> _transferRequest;
        private readonly Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> _transferDispatch;
        private readonly Func<SessionRequestMessage, Task<SessionReplyMessage>> _sessionRequest;

        public VaspMessageHandlerCallbacks(
            Func<SessionRequestMessage, Task<SessionReplyMessage>> sessionRequest,
            Func<TransferRequestMessage, Task<TransferReplyMessage>> transferRequest,
            Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> transferDispatch)
        {
            _sessionRequest= sessionRequest ?? throw new ArgumentNullException(nameof(sessionRequest));
            _transferRequest = transferRequest ?? throw new ArgumentNullException(nameof(transferRequest));
            _transferDispatch = transferDispatch ?? throw new ArgumentNullException(nameof(transferDispatch));
        }

        public Task<SessionReplyMessage> SessionRequestHandlerAsync(SessionRequestMessage request)
        {
            return _sessionRequest?.Invoke(request);
        }

        public Task<TransferReplyMessage> TransferRequestHandlerAsync(TransferRequestMessage request)
        {
            return _transferRequest?.Invoke(request);
        }

        public Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request)
        {
            return _transferDispatch?.Invoke(request);
        }
    }
}
