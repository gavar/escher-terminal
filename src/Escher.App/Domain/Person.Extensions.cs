namespace Escher.App.Domain
{
    public static class PersonExtensions
    {
        /// <summary> Set <see cref="Person.Spouse"/> to a given value. </summary>
        /// <returns>Input <paramref name="person"/> itself.</returns>
        public static Person WithSpouse(this Person person, Person spouse)
        {
            person.Spouse = spouse;
            return person;
        }

        /// <summary> Set bidirectional <see cref="Person.Spouse"/> relationships of the given persons. </summary>
        /// <returns>Input <paramref name="person"/> itself.</returns>
        public static Person AsFamily(this Person person, Person spouse)
        {
            person.Spouse = spouse;
            spouse.Spouse = person;
            return person;
        }
    }
}