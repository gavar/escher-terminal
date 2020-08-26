using System.Collections.Generic;

namespace Escher.Data
{
    /// <summary>
    /// Represents object which may be persisted to the database.
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Primary key of the object.
        /// </summary>
        object? Id { get; }

        /// <summary>
        /// Check if this entity is a new instance.
        /// By default entity is considered as a new if primary key is not assigned.
        /// </summary>
        bool IsNew => Id == null;
    }

    /// <summary>
    /// Generic version of <see cref="IPersistable"/> having strongly typed primary key.
    /// </summary>
    /// <typeparam name="ID">Type of the primary key.</typeparam>
    public interface IPersistable<ID> : IPersistable
    {
        /// <inheritdoc cref="IPersistable.Id"/>
        new ID Id { get; set; }

        /// <inheritdoc cref="IPersistable.IsNew"/>
        new bool IsNew => EqualityComparer<ID>.Default.Equals(Id, default);
    }
}