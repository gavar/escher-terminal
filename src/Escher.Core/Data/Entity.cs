using FlatFile.Delimited.Attributes;

namespace Escher.Data
{
    /** Base class for entities having 64 bit primary key. */
    public class Entity : IPersistable<long>
    {
        [DelimitedField(1, Name = nameof(Id))]
        public long Id { get; set; }

        public bool IsNew => Id == default;

        object IPersistable.Id => Id;
    }
}