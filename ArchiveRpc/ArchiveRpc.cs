using System;
using System.Collections.Generic;
using System.Linq;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.VM;
using static System.IO.Path;
using Neo.Cryptography.ECC;
using Neo.SmartContract.Iterators;
using Neo.SmartContract.Native;
using Neo.VM.Types;
using Neo.Wallets;
using System.IO;

namespace Neo.Plugins
{
    public class ArchiveRpc : Plugin
    {
        protected override void OnSystemLoaded(NeoSystem system) => RpcServerPlugin.RegisterMethods(this, system.Settings.Network);


        [RpcMethod]
        protected virtual JObject GetStorageAtHeight(JArray _params)
        {
            int contractId = int.Parse(_params[0].AsString());
            byte[] prefix = Convert.FromBase64String(_params[1].AsString());
            uint blockheight = uint.Parse(_params[2].AsString());
           
            var key = ArchiveStore.UintToBytes((uint)contractId).Reverse().Concat(prefix).ToArray();
            
            JObject json = new();
            json["value"] = Convert.ToBase64String(ArchiveStore.GetStorageAtHeight(key, blockheight));
            return json;
        }
    }
}

/*
curl -d '{
    "jsonrpc": "2.0",
    "method": "getstorageatheight",
    "params": ["-4","DA==","1"],
    "id": 1
}' http://127.0.0.1:10332 |
 json_pp
 */
