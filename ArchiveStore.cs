using System;
using System.Collections.Generic;
using System.Linq;
using Neo.IO;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;

namespace Neo.Plugins
{
    public class ArchiveStore : Plugin, IPersistencePlugin
    {
        IStore store;
        public override void Dispose() => store.Dispose();
        protected override void OnSystemLoaded(NeoSystem system) => store = system.LoadStore(String.Format("Data_Archive_{0}", system.Settings.Network.ToString("X")));
        public void OnPersist(NeoSystem system, Block block, DataCache snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList) => snapshot.GetChangeSet().ToList().ForEach(v => store.Put(v.Key.ToArray().Concat(BitConverter.IsLittleEndian ? BitConverter.GetBytes(block.Index).Reverse() : BitConverter.GetBytes(block.Index)).ToArray(), v.Item.ToArray().Append((byte)v.State).ToArray()));
    }
}
