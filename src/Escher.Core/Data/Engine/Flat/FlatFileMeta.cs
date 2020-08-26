using System.Text.Json;

namespace Escher.Data.Engine.Flat
{
    public class FlatFileMeta
    {
        /// <summary>
        /// JSON serialization settings to use while persisting meta information.
        /// </summary>
        public static readonly JsonSerializerOptions SERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        /// <summary>
        /// The seed value is the value inserted into an identity column for the first row loaded into the table.
        /// All subsequent rows contain the current identity value plus the increment value where current identity value is the
        /// last identity value generated for the table or view.
        /// </summary>
        public long Seed;
    }
}