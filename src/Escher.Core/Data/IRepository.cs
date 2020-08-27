using System;

namespace Escher.Data
{
    public interface IRepository : IDisposable
    {
        /// <summary> Stub repository doing nothing. </summary>
        static readonly IRepository Null = IRepository<object>.Null;

        /// <summary>
        /// Commit changes to the underlying data store.
        /// </summary>
        void Commit();

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            // empty
        }
    }

    /// <summary>
    /// Gateway for CRUD operations over the entity of the particular type.
    /// </summary>
    /// <typeparam name="T">Type of the entity this repository operates on.</typeparam>
    public interface IRepository<T> : IRepository
    {
        /// <summary> Stub repository doing nothing. </summary>
        static readonly IRepository<T> Null = new NullRepository<T>();

        /// <summary>
        /// Inserts a new entity.
        /// Use the returned instance for further operations as it might be changed completely.
        /// </summary>
        /// <param name="entity">the object to insert.</param>
        /// <returns>The inserted entity.</returns>
        /// <exception cref="EntityExistsException">when entity with the same primary key already exists.</exception>
        T Insert(T entity);
    }

    internal class NullRepository<T> : IRepository<T>
    {
        public T Insert(T entity)
        {
            return entity;
        }

        public void Commit() { }
    }
}