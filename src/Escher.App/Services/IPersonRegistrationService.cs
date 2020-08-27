using System;
using Escher.App.Domain;
using Escher.Utils;
using static Escher.App.Domain.PersonVerifyStatus;

namespace Escher.App.Services
{
    public interface IPersonRegistrationService
    {
        static readonly int MinAge = 16;
        static readonly int AdultAge = 18;

        public void Register(Person person)
        {
            AssertPerson(person);

            if (person.Spouse != null)
                AssertPerson(person.Spouse, "Spouse");

            DoRegister(person);
        }

        public int VerifyAge(DateTime dob, bool authorized)
        {
            if (DateUtils.AgeFullYears(dob) < MinAge)
                return Young;

            if (DateUtils.AgeFullYears(dob) < AdultAge && !authorized)
                return Unauthorized;

            return 0;
        }

        public int Verify(Person person)
        {
            var status = VerifyAge(person.BirthDate, person.IsAuthorized);
            if (status < 0)
                return status;

            // referencing self
            var spouse = person.Spouse;
            if (spouse == person)
                return Invalid;

            // referencing wrong person
            if (spouse != null && spouse.Spouse != person)
                return Invalid;

            return 0;
        }

        public string VerificationMessage(int status, string subject = "Person")
        {
            return status switch
            {
                0 => null,
                Young => $"{subject} have to be at least {MinAge} years old",
                Unauthorized => $"{subject} requires parents authorization until age of {AdultAge}",
                Invalid => $"{subject} has invalid state",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }

        protected void AssertPerson(Person person, string subject = "Person")
        {
            if (person.Spouse == person)
                throw new ArgumentException($"{subject} references itself");

            var status = Verify(person);
            if (status < 0)
                throw new ArgumentException(VerificationMessage(status, subject));
        }

        protected void DoRegister(Person person);
    }
}