using System.Data.Entity.Core;
using FlatFile.Delimited;
using FlatFile.Delimited.Implementation;

namespace Escher.Data.Engine.Flat
{
    public class FlatFileRepositoryLineBuilder : DelimitedLineBuilder
    {
        public FlatFileRepositoryLineBuilder(IDelimitedLayoutDescriptor descriptor) : base(descriptor) { }

        protected override string GetStringValueFromField(IDelimitedFieldSettingsContainer field, object fieldValue)
        {
            if (fieldValue is IPersistable entity)
            {
                var id = entity.Id;
                if (entity.IsNew || id == null)
                    // TODO: better message
                    throw new EntityException("Trying to persist transient entity, save it first!");

                return entity.Id.ToString();
            }

            return field.TypeConverter != null
                ? field.TypeConverter.ConvertToString(fieldValue)
                : base.GetStringValueFromField(field, fieldValue);
        }
    }
}