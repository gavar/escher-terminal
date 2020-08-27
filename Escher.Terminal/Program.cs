using System;
using Escher.App;
using Escher.App.Domain;
using Escher.App.Services;
using Escher.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Escher.Terminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            new ApplicationBundle().Install(services);
            services.AddSingleton<Program>();

            var provider = services.BuildServiceProvider();
            var program = provider.GetRequiredService<Program>();
            program.Run();
        }

        private readonly IRepositoryCentral central;
        private readonly IPersonRegistrationService service;

        public Program(IRepositoryCentral central, IPersonRegistrationService service)
        {
            this.service = service;
            this.central = central;
        }

        public void Run()
        {
            var spouse = new Person
            {
                FirstName = "Janet",
                LastName = "Doe",
                Authorized = true,
                BirthDate = new DateTime(2004, 06, 03),
            };
            var person = new Person
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1991, 09, 13),
                Spouse = spouse,
            };
            
            // TODO: validate spouse relations by a service
            spouse.Spouse = person;
            
            service.Register(person);
            central.Commit();
            Console.ReadKey();
        }
    }
}