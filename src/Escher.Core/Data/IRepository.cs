using System;

namespace Escher.Data
{
    /// <summary>
    /// Gateway for CRUD operations over the entity of the particular type.
    /// </summary>
    /// <typeparam name="T">Type of the entity this repository operates on.</typeparam>
    public interface IRepository<T> : IDisposable
    {
        /// <summary>
        /// Inserts a new entity.
        /// Use the returned instance for further operations as it might be changed completely.
        /// </summary>
        /// <param name="entity">the object to insert.</param>
        /// <returns>The inserted entity.</returns>
        /// <exception cref="EntityExistsException">when entity with the same primary key already exists.</exception>
        T Insert(T entity);

        /// <summary>
        /// Commit changes to the underlying cdata store.
        /// </summary>
        void Commit();

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            // empty
        }
    }
}