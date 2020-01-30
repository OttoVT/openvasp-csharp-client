using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages;
using OpenVASP.Messaging.Messages.Entities;
using Xunit;
using Xunit.Abstractions;

namespace OpenVASP.Tests
{
    public class ClientExample
    {
        private readonly ITestOutputHelper testOutputHelper;

        public INodeClient NodeClient { get; set; }

        public VaspTestSettings Settings { get; set; }

        public ClientExample(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            NodeClient = null;
            Settings = new VaspTestSettings();
        }

        [Fact]
        public async Task CreateVaspForNaturalPerson_Builder()
        {
            var builder = new VaspInformationBuilder(NodeClient.EthereumRpc);

            VaspInformation vaspInfo = await builder.CreateForNaturalPersonAsync(
                Settings.VaspSmartContractAddressPerson,
                Settings.NaturalPersonIds,
                Settings.PlaceOfBirth);

            VaspClient vasp = VaspClient.Create(
                vaspInfo, 
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
            VaspInformation vaspInfo = await VaspInformationBuilder.CreateForNaturalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressPerson,
                Settings.NaturalPersonIds,
                Settings.PlaceOfBirth);
            
            VaspClient vasp = VaspClient.Create(
                vaspInfo,
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
            VaspInformation vaspInfo = await VaspInformationBuilder.CreateForJuridicalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressJuridical,
                Settings.JuridicalIds);

            VaspClient vasp = VaspClient.Create(vaspInfo,
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
            VaspInformation vaspInfo = await VaspInformationBuilder.CreateForJuridicalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressBank,
                Settings.Bic);

            VaspClient vasp = VaspClient.Create(vaspInfo,
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
            VaspInformation vaspInfoPerson = await VaspInformationBuilder.CreateForNaturalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressPerson,
                Settings.NaturalPersonIds,
                Settings.PlaceOfBirth);

            VaspClient originator = VaspClient.Create(
                vaspInfoPerson,
                Settings.PersonHandshakePrivateKeyHex,
                Settings.PersonSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            VaspInformation vaspInfoJuridical = await VaspInformationBuilder.CreateForJuridicalPersonAsync(
                NodeClient.EthereumRpc,
                Settings.VaspSmartContractAddressJuridical,
                Settings.JuridicalIds);

            VaspClient beneficiary = VaspClient.Create(vaspInfoJuridical,
                Settings.JuridicalHandshakePrivateKeyHex,
                Settings.JuridicalSignaturePrivateKeyHex,
                NodeClient.EthereumRpc,
                NodeClient.WhisperRpc);

            var beneficiaryVaan = VirtualAssetssAccountNumber.Create(vaspInfoPerson.GetVaspCode(), "524ee3fb082809");

            IVaspMessageHandler messageHandler = new VaspMessageHandlerCallbacks(
                request => Task.FromResult((TransferReply)null),
                dispatch => Task.FromResult((TransferConfirmationMessage)null));

            originator.RunListener(messageHandler);
            beneficiary.RunListener(messageHandler);

            // change enum to string and add constants
            //beneficiary.TransferRequest += request => new TransferReply(VirtualAssetType.ETH, TransferType.BlockchainTransfer, "10", "1223");
            //beneficiary.TransferDispatch += message => new TransferConfirmationMessage();

            VaspSession session = await originator.CreateSessionAsync(beneficiaryVaan.VaspCode);

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

    public interface IVaspMessageHandler
    {
        Task<TransferReply> TransferRequestHandlerAsync(TransferRequest request);

        Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request);
    }

    public class VaspMessageHandlerCallbacks : IVaspMessageHandler
    {
        private readonly Func<TransferRequest, Task<TransferReply>> _transferRequest;
        private readonly Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> _transferDispatch;

        public VaspMessageHandlerCallbacks(
            Func<TransferRequest, Task<TransferReply>> transferRequest,
            Func<TransferDispatchMessage, Task<TransferConfirmationMessage>> transferDispatch)
        {
            _transferRequest = transferRequest ?? throw new ArgumentNullException(nameof(transferRequest));
            _transferDispatch = transferDispatch ?? throw new ArgumentNullException(nameof(transferDispatch));
        }

        public Task<TransferReply> TransferRequestHandlerAsync(TransferRequest request)
        {
            return _transferRequest?.Invoke(request);
        }

        public Task<TransferConfirmationMessage> TransferDispatchHandlerAsync(TransferDispatchMessage request)
        {
            return _transferDispatch?.Invoke(request);
        }
    }

    public class VaspSession
    {
        public object SessionId { get; set; }
        public bool IsOriginator { get; set; }

        public bool IsBeneficiary { get; set; }
        public VaspSessionCounterparty Counterparty { get; set; }


        public async Task<TransferReply> TransferRequestAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TransferConfirmationMessage> TransferDispatchAsync()
        {
            throw new NotImplementedException();
        }

        public async Task Termination()
        {
            throw new NotImplementedException();
        }
    }

    public class VaspSessionCounterparty
    {
        public object VaspInfo { get; set; }
        public object Vaan { get; set; }
    }

    public class VaspInformationBuilder
    {
        public VaspInformationBuilder(IEthereumRpc nodeClientEthereumRpc)
        {
            throw new NotImplementedException();
        }

        public Task<VaspInformation> CreateForNaturalPersonAsync(
            object vaspSmartContractAddress,
            NaturalPersonId[] settingsNaturalPersonIds,
            object settingsDateOfBirth)
        {
            throw new NotImplementedException();
        }

        public Task<VaspInformation> CreateForJuridicalPersonAsync(
            object VaspSmartContractAddress,
            object settingsJuridicalIds)
        {
            throw new NotImplementedException();
        }

        public Task<VaspInformation> CreateForBankAsync(
            object vaspSmartContractAddress,
            object settingsVaspSmartContractAddress,
            IEnumerable<char> settingsBic)
        {
            throw new NotImplementedException();
        }

        public static Task<VaspInformation> CreateForNaturalPersonAsync(
            IEthereumRpc ethereumRpc,
            object vaspSmartContractAddress, 
            NaturalPersonId[] settingsNaturalPersonIds,
            object settingsDateOfBirth)
        {
            throw new NotImplementedException();
        }

        public static Task<VaspInformation> CreateForJuridicalPersonAsync(
            IEthereumRpc ethereumRpc,
            object VaspSmartContractAddress, 
            object settingsJuridicalIds)
        {
            throw new NotImplementedException();
        }

        public static Task<VaspInformation> CreateForBankAsync(
            IEthereumRpc ethereumRpc,
            object vaspSmartContractAddress,
            object settingsVaspSmartContractAddress, 
            IEnumerable<char> settingsBic)
        {
            throw new NotImplementedException();
        }
    }

    public interface IEthereumRpc
    {
    }

    public interface INodeClient
    {
        IEthereumRpc EthereumRpc { get; }
        IWhisperRpc WhisperRpc { get; set; }
    }

    public interface IWhisperRpc
    {
    }

    public class VaspClient : IDisposable
    {
        public VaspInformation VaspInfo { get; set; }
        public event Func<TransferDispatchMessage, TransferConfirmationMessage> TransferDispatch;

        public event Func<TransferRequest, TransferReply> TransferRequest;

        public event Action<VaspInformation> SessionTerminated;

        public event Action<VaspInformation, VaspSession> SessionCreated;
        
        public void RunListener(IVaspMessageHandler messageHandler)
        {
            throw new NotImplementedException();
        }

        public async Task<VaspSession> CreateSessionAsync(object beneficiaryVaan)
        {
            throw new NotImplementedException();
        }

        public static VaspClient Create(
            VaspInformation vaspInfo,
            string handshakePrivateKeyHex,
            string signaturePrivateKeyHex,
            IEthereumRpc nodeClientEthereumRpc, 
            IWhisperRpc nodeClientWhisperRpc)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IReadOnlyList<VaspSession> GetActiveSessions()
        {
            throw new NotImplementedException();
        }
    }

    public class VaspTestSettings
    {
        public object NodeRPC;
        public string VaspSmartContractAddressPerson { get; set; }
        public string VaspSmartContractAddressJuridical { get; set; }
        public string VaspSmartContractAddressBank { get; set; }

        public NaturalPersonId[] NaturalPersonIds { get; set; }
        public object PlaceOfBirth { get; set; }
        public JuridicalPersonId[] JuridicalIds { get; set; }
        public IEnumerable<char> Bic { get; set; }

        public string PersonHandshakePrivateKeyHex { get; set; }
        public string PersonSignaturePrivateKeyHex { get; set; }
        public string JuridicalHandshakePrivateKeyHex { get; set; }
        public string JuridicalSignaturePrivateKeyHex { get; set; }
        public string BankHandshakePrivateKeyHex { get; set; }
        public string BankSignaturePrivateKeyHex { get; set; }
    }
}
