using FlatFile.Delimited;

namespace Escher.Data.Engine.Flat
{
    /// <summary>
    /// Flat file repository with 64 bit signed key.
    /// </summary>
    public class FlatFileRepository64<T> : FlatFileRepository<T, long> where T : class, IPersistable<long>, new()
    {
        public FlatFileRepository64(string path) : base(path) { }
        public FlatFileRepository64(IDelimitedLayoutDescriptor descriptor, string path) : base(path, descriptor) { }

        protected override long NextKey()
        {
            return ++meta.Seed;
        }

        protected override void GrowSeed(long key)
        {
            if (key > meta.Seed)
                meta.Seed = key;
        }
    }
}