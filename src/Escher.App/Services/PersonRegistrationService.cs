using Escher.App.Data;
using Escher.App.Domain;

namespace Escher.App.Services
{
    public class PersonRegistrationService : IPersonRegistrationService
    {
        protected readonly IPersonRepository repository;

        public PersonRegistrationService(IPersonRepository repository)
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