using System;
using System.Collections.Generic;
using System.Linq;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Plugins
{
    public class ArchiveStore : Plugin, IPersistencePlugin
    {
        public void OnPersist(NeoSystem system, Block block, DataCache snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList) => snapshot.GetChangeSet().ToList().ForEach(v => snapshot.Add(new StorageKey() { Id = int.MinValue, Key = v.Key.ToArray().Concat(BitConverter.IsLittleEndian ? BitConverter.GetBytes(block.Index).Reverse() : BitConverter.GetBytes(block.Index)).ToArray() }, new StorageItem(v.Item.ToArray().Append((byte)v.State).ToArray())));
    }
}
