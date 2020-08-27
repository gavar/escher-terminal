using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Escher
{
    public static class Pool
    {
        public static readonly DefaultObjectPoolProvider Provider = new DefaultObjectPoolProvider();
        public static readonly ObjectPool<StringBuilder> StringBuilders = Provider.CreateStringBuilderPool();

        public static ObjectRent<T> Rent<T>(this ObjectPool<T> pool, out T value) where T : class
        {
            return new ObjectRent<T>(pool, out value);
        }

        public static ArrayRent<T> Rent<T>(int capacity)
        {
            return new ArrayRent<T>(capacity);
        }
    }
}