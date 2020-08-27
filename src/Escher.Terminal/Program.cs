using System;
using Escher.App;
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
            central.Commit();
            Console.ReadKey();
        }
    }
}