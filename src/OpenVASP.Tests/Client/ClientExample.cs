using System.Collections.Generic;
using System.Linq;
using Nethereum.Signer;
using OpenVASP.CSharpClient;
using OpenVASP.CSharpClient.Cryptography;
using OpenVASP.Messaging.Messages.Entities;
using Xunit;
using Xunit.Abstractions;

namespace OpenVASP.Tests
{
    public class ClientExample
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ClientExample(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void SessionCreationTest()
        {
            var settings = new VaspTestSettings();
            VaspInstance naturalIdVasp = VaspInstance.CreateForNaturalId(
                settings.VaspSmartContractAddress, 
                settings.NaturalPersonIds, 
                settings.DateOfBirth);

            VaspInstance juridicalIdVasp = VaspInstance.CreateForJuridicalId(
                settings.VaspSmartContractAddress,
                settings.JuridicalIds);

            VaspInstance bicIdVasp = VaspInstance.CreateForBic(
                settings.VaspSmartContractAddress,
                settings.Bic);

            VaspInformation vaspInfo = naturalIdVasp.VaspInfo;
            Assert.NotNull(vaspInfo.NaturalPersonIds);

            Assert.True(settings.NaturalPersonIds.SequenceEqual(vaspInfo.NaturalPersonIds));
            Assert.True(settings.JuridicalPersonIds.SequenceEqual(vaspInfo.JuridicalPersonIds));
            Assert.Equal(settings.Bic, vaspInfo.BIC);
        }
    }
}
