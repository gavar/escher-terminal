using System;
using System.Buffers;

namespace Escher
{
    public static class Arrays<T>
    {
        /// <summary> Array with zero length which may be used as a null-safe stub. </summary>
        public static readonly T[] Empty = new T[0];

        /// <summary>
        /// Flag identifies whether array contents may produce a memory leak if left uncleared.
        /// Set to true by default for non primitive elements but left accessible for manual adjustment.
        /// </summary>
        public static bool Leaking = !typeof(T).IsPrimitive;

        /// <summary>
        /// Grow array rented from <see cref="ArrayPool{T}.Shared" /> array pool to surely fit provided number of elements.
        /// </summary>
        /// <param name="array">Array with elements rented from the array pool.</param>
        /// <param name="size">Number of elements occupied by the array.</param>
        /// <param name="capacity">The minimum length of the array needed.</param>
        /// <returns>Same or newly rented array with copied elements.</returns>
        public static T[] Inflate(T[] array, int size, int capacity)
        {
            // check if requires to grow
            if (array.Length < capacity)
                return array;

            // TODO: check if pool has power of 2 grow policy 
            var next = ArrayPool<T>.Shared.Rent(capacity);

            // copy elements
            if (size > 0)
                Array.Copy(array, 0, next, 0, size);

            // return previous array
            if (array.Length > 0)
            {
                // make sure to return clear array
                if (Leaking && size > 0)
                    Array.Clear(array, 0, size);

                ArrayPool<T>.Shared.Return(array);
            }

            return next;
        }

        /// <summary>
        /// Return array to the <see cref="ArrayPool{T}.Shared" /> array pool.
        /// Array will be cleared if <see cref="Leaking" /> is set to true.
        /// The <paramref name="size" /> will be set to zero upon completion.
        /// </summary>
        /// <param name="array">Array to return to the pool.</param>
        /// <param name="size">Number of elements occupied by the array.</param>
        /// <returns></returns>
        public static bool Return(ref T[] array, ref int size)
        {
            // empty arrays are immutable, leave as is
            if (array.Length > 0)
            {
                // clear elements
                if (Leaking && size > 0)
                    Array.Clear(array, 0, size);

                // return back to the pool
                ArrayPool<T>.Shared.Return(array);

                // replace with empty immutable array
                size = 0;
                array = Empty;
                return true;
            }

            // always reset size even if empty array
            // helps to recover from invalid state
            size = 0;
            return false;
        }
    }
}