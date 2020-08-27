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

        public int Verify(Person person)
        {
            if (DateUtils.AgeFullYears(person.BirthDate) >= MinAge)
                return Young;

            if (DateUtils.AgeFullYears(person.BirthDate) < AdultAge && !person.IsAuthorized)
                return Unauthorized;

            return 0;
        }

        public string VerificationMessage(int status, string subject = "Person")
        {
            return status switch
            {
                0 => null,
                Young => $"{subject} have to be at least {MinAge} years old",
                Unauthorized => $"{subject} requires parents authorization until age of {AdultAge}",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }

        protected void AssertPerson(Person person, string subject = "Person")
        {
            var status = Verify(person);
            if (status < 0)
                throw new ArgumentException(VerificationMessage(status, subject));
        }

        protected void DoRegister(Person person);
    }
}