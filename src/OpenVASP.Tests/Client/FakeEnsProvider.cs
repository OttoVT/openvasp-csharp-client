﻿using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using OpenVASP.Messaging.Messages.Entities;

namespace OpenVASP.Tests.Client
{
    public class FakeEnsProvider : IEnsProvider
    {
        private readonly Dictionary<string, string> _addressDictionary = new Dictionary<string, string>()
        {
            {"7dface61", "0x6befaf0656b953b188a0ee3bf3db03d07dface61"},
            {"bbb4ee5c", "0x08fda931d64b17c3acffb35c1b3902e0bbb4ee5c"}
        };

        public FakeEnsProvider()
        {
        }

        public Task<string> GetContractAddressByVaspCodeAsync(VaspCode vaspCode)
        {
            _addressDictionary.TryGetValue(vaspCode.Code.ToLower(CultureInfo.InvariantCulture), out var result);

            return Task.FromResult(result);
        }
    }
}