using System;
using Escher.App.Domain;
using Escher.Data;
using Xunit;
using Xunit.Abstractions;
using static Escher.App.Domain.PersonVerifyStatus;

namespace Escher.App.Services
{
    public class PersonRegistrationServiceTest
    {
        private static readonly DateTime Now = DateTime.Now;

        private readonly ITestOutputHelper output;
        private readonly IPersonRegistrationService service;

        public PersonRegistrationServiceTest(ITestOutputHelper output)
        {
            this.output = output;
            service = new PersonRegistrationService(IRepository<Person>.Null);
        }

        [Theory]
        [MemberData(nameof(PersonVerifications))]
        public void Verify(Person person, int expected)
        {
            var actual = service.Verify(person);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(PersonVerifications))]
        [MemberData(nameof(PersonRegistrations))]
        public void Register(Person person, int expected)
        {
            if (expected == 0) Action();
            else Assert.Throws<ArgumentException>(Action);

            void Action()
            {
                service.Register(person);
            }
        }
        
        [Theory]
        [MemberData(nameof(InvalidPersons))]
        public void VerifyInvalid(Person person, string message)
        {
            output.WriteLine(message);
            var status = service.Verify(person);
            Assert.Equal(status, Invalid);
        }

        [Theory]
        [MemberData(nameof(InvalidPersons))]
        public void RegisterInvalid(Person person, string message)
        {
            output.WriteLine(message);
            Assert.Throws<ArgumentException>(() => service.Register(person));
        }

        public static TheoryData<Person, int> PersonVerifications()
        {
            return new TheoryData<Person, int>
            {
                { Y(15), Young },
                { Y(15).AsFamily(Y(20)), Young },
                
                { Y(16), Unauthorized },
                { Y(17), Unauthorized },
                { Y(17).AsFamily(Y(20)), Unauthorized },

                { Y(20), 0 },
                { Y(16, true), 0 },
                { Y(16, true).AsFamily(Y(20)), 0 },
                { Y(16, true).AsFamily(Y(16, true)), 0 },
            };
        }

        public static TheoryData<Person, int> PersonRegistrations()
        {
            return new TheoryData<Person, int>
            {
                { Y(20).AsFamily(Y(15)), Young },
                { Y(20).AsFamily(Y(17)), Unauthorized },
                { Y(20).AsFamily(Y(20)), 0 },
            };
        }

        public static TheoryData<Person, string> InvalidPersons()
        {
            var circular = Y(20);
            circular.Spouse = circular;

            return new TheoryData<Person, string>
            {
                { Y(20).WithSpouse(Y(20)), "should have bidirectional relation with spouse" },
                { Y(20).WithSpouse(Y(20).AsFamily(Y(20))), "spouse references wrong person" },
                { Y(20).WithSpouse(circular), "spouse cannot reference itself" },
                { circular, "cannot have spouse as self reference" },
            };
        }


        /// <summary> Create new person instance with the given age. </summary>
        private static Person Y(int years, bool? authorized = null)
        {
            return new Person
            {
                BirthDate = Now.AddYears(-years),
                Authorized = authorized,
            };
        }
    }
}