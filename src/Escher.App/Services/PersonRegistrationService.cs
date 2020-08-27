using Escher.App.Domain;
using Escher.Data;

namespace Escher.App.Services
{
    public class PersonRegistrationService : IPersonRegistrationService
    {
        protected readonly IRepository<Person> repository;

        public PersonRegistrationService(IRepository<Person> repository)
        {
            this.repository = repository;
        }

        void IPersonRegistrationService.DoRegister(Person person)
        {
            repository.Insert(person);
            if (person.Spouse != null)
                repository.Insert(person.Spouse);
        }
    }
}