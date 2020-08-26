using Microsoft.Extensions.ObjectPool;

namespace Escher
{
    /// <summary>
    /// Represents a pooled object instance being returned back to the pool upon disposing.
    /// </summary>
    public readonly ref struct ObjectRent<T> where T : class
    {
        public readonly T instance;
        private readonly ObjectPool<T> pool;

        public ObjectRent(ObjectPool<T> pool) : this()
        {
            this.pool = pool;
            instance = pool.Get();
        }

        public ObjectRent(ObjectPool<T> pool, out T instance) : this()
        {
            this.pool = pool;
            this.instance = instance = pool.Get();
        }

        public void Dispose()
        {
            pool.Return(instance);
        }
    }
}