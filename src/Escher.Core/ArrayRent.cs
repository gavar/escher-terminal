using System.Buffers;
using System.Runtime.CompilerServices;

namespace Escher
{
    /// <summary>
    /// Rents underlying array from <see cref="ArrayPool{T}.Shared" /> array pool upon growing.
    /// Handy to use for temporary collections.
    /// Provides very basic but optimized operations.
    /// Returns array back to the <see cref="ArrayPool{T}.Shared" /> upon disposing.
    /// </summary>
    public ref struct ArrayRent<T>
    {
        public int size;
        public T[] array;

        public ArrayRent(int capacity = 0)
        {
            size = 0;
            array = capacity > 0
                ? ArrayPool<T>.Shared.Rent(capacity)
                : Arrays<T>.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            Grow(size++);
            array[size] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            array[size++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Clear()
        {
            return Arrays<T>.Return(ref array, ref size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Grow(int capacity)
        {
            array = Arrays<T>.Inflate(array, size, capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Arrays<T>.Return(ref array, ref size);
        }
    }
}