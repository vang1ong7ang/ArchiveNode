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
        public void OnPersist(NeoSystem ns, Block block, DataCache snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> ae) => snapshot.GetChangeSet().ToList().ForEach(v => store.Put(CompositeKey(v.Key.ToArray(), block.Index), v.Item.ToArray().Append((byte)v.State).ToArray()));
        public byte[] GetStorageAtHeight(byte[] key, uint height) => ExtractStorage(key, store.Seek(CompositeKey(key, height), SeekDirection.Backward).FirstOrDefault());
        static byte[] ExtractStorage(byte[] key, (byte[] Key, byte[] Value) kv) => kv.Key.SkipLast(4).SequenceEqual(key) ? kv.Value.Count() > 0 && kv.Value.TakeLast(1).Single() != ((byte)TrackState.Deleted) ? kv.Value.SkipLast(1).ToArray() : null : null;
        static byte[] CompositeKey(byte[] key, uint n) => key.Concat(UintToBytes(n)).ToArray();
        static IEnumerable<byte> UintToBytes(uint n) => BitConverter.IsLittleEndian ? BitConverter.GetBytes(n).Reverse() : BitConverter.GetBytes(n);
    }
}
