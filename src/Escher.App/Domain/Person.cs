using System;
using Escher.Data;
using FlatFile.Delimited.Attributes;

namespace Escher.App.Domain
{
    [DelimitedFile]
    public class Person : Entity
    {
        [DelimitedField(2)]
        public string FirstName { get; set; }

        [DelimitedField(3)]
        public string LastName { get; set; }

        [DelimitedField(4)]
        public DateTime BirthDate { get; set; }

        [DelimitedField(5)]
        public bool? Authorized { get; set; }

        [DelimitedField(6)]
        public Person Spouse { get; set; }

        public bool IsAuthorized => Authorized.GetValueOrDefault(false);
    }
}