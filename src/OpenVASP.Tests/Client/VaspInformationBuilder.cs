using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenVASP.CSharpClient;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public class VaspInformationBuilder
    {
        private readonly IEthereumRpc _nodeClientEthereumRpc;

        public VaspInformationBuilder(IEthereumRpc nodeClientEthereumRpc)
        {
            this._nodeClientEthereumRpc = nodeClientEthereumRpc;
        }

        public async Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForNaturalPersonAsync(
            string vaspSmartContractAddress,
            NaturalPersonId[] naturalPersonIds,
            PlaceOfBirth placeOfBirth)
        {
            var vaspContractInfo = await _nodeClientEthereumRpc.GetVaspContractInfoAync(vaspSmartContractAddress);
            var vaspInformation = new VaspInformation(
                vaspContractInfo.Name,
                vaspSmartContractAddress,
                vaspContractInfo.HandshakeKey,//Ensure it is correct
                vaspContractInfo.Address,
                placeOfBirth,
                naturalPersonIds,
                null);

            return (vaspInformation, vaspContractInfo);
        }

        public async Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForJuridicalPersonAsync(
            string vaspSmartContractAddress,
            JuridicalPersonId[] juridicalIds)
        {
            var vaspContractInfo = await _nodeClientEthereumRpc.GetVaspContractInfoAync(vaspSmartContractAddress);
            var vaspInformation = new VaspInformation(
                vaspContractInfo.Name,
                vaspSmartContractAddress,
                vaspContractInfo.HandshakeKey,//Ensure it is correct
                vaspContractInfo.Address,
                null,
                null,
                juridicalIds);

            return (vaspInformation, vaspContractInfo);
        }

        public Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForBankAsync(
            string vaspSmartContractAddress,
            IEnumerable<char> settingsBic)
        {
            throw new NotImplementedException();
        }

        public static Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForNaturalPersonAsync(
            IEthereumRpc ethereumRpc,
            string vaspSmartContractAddress,
            NaturalPersonId[] settingsNaturalPersonIds,
            PlaceOfBirth settingsPlaceOfBirth)
        {
            var vaspInformationBuilder = new VaspInformationBuilder(ethereumRpc);

            return vaspInformationBuilder.CreateForNaturalPersonAsync(vaspSmartContractAddress, settingsNaturalPersonIds,
                settingsPlaceOfBirth);
        }

        public static Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForJuridicalPersonAsync(
            IEthereumRpc ethereumRpc,
            string vaspSmartContractAddress,
            JuridicalPersonId[] juridicalIds)
        {
            var vaspInformationBuilder = new VaspInformationBuilder(ethereumRpc);

            return vaspInformationBuilder.CreateForJuridicalPersonAsync(vaspSmartContractAddress, juridicalIds);
        }

        public static Task<(VaspInformation VaspInformation, VaspContractInfo VaspContractInfo)> CreateForBankAsync(
            IEthereumRpc ethereumRpc,
            string vaspSmartContractAddress,
            IEnumerable<char> settingsBic)
        {
            throw new NotImplementedException();
        }
    }
}