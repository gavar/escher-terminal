using System;
using System.IO;
using Escher.App.Domain;
using Escher.App.Services;
using Escher.Data;
using Escher.Data.Engine.Flat;
using Microsoft.Extensions.DependencyInjection;

namespace Escher.App
{
    public class ApplicationBundle
    {
        private const string DB = "db";

        public void Install(IServiceCollection services)
        {
            Directory.CreateDirectory(DB);
            services.AddSingleton<IRepositoryCentral, RepositoryServiceCentral>();
            services.AddSingleton(CreatePersonRepository);
            services.AddSingleton(CreatePersonRegistrationService);
        }

        private static IRepository<Person> CreatePersonRepository(IServiceProvider provider)
        {
            return new FlatFileRepository64<Person>(DB + "/person.csv");
        }

        private static IPersonRegistrationService CreatePersonRegistrationService(IServiceProvider provider)
        {
            var central = provider.GetRequiredService<IRepositoryCentral>();
            var repository = central.GetRepository<IRepository<Person>>();
            return new PersonRegistrationService(repository);
        }
    }
}