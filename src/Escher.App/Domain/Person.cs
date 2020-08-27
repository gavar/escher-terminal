using System;
using Escher.Data;
using Escher.Data.Engine.Flat;
using FlatFile.Delimited.Attributes;

namespace Escher.App.Domain
{
    [DelimitedFile(Delimiter = ", ", HasHeader = true)]
    public class Person : Entity
    {
        [DelimitedField(2, Name = nameof(FirstName))]
        public string FirstName { get; set; }

        [DelimitedField(3, Name = nameof(LastName))]
        public string LastName { get; set; }

        [DelimitedField(4, Name = nameof(BirthDate), Converter = typeof(ShortDateFormat))]
        public DateTime BirthDate { get; set; }

        [DelimitedField(5, Name = nameof(Authorized))]
        public bool? Authorized { get; set; }

        [DelimitedField(6, Name = nameof(Spouse))]
        public Person Spouse { get; set; }

        public bool IsAuthorized => Authorized.GetValueOrDefault(false);
    }
}