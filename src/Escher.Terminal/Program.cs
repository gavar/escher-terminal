using System;
using System.Collections.Generic;
using Escher.App;
using Escher.App.Domain;
using Escher.App.Services;
using Escher.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Escher.Terminal
{
    internal class Program
    {
        private const string PERSON = "Person";
        private const string SPOUSE = "Spouse";
        private const string REGISTRATION = "Registration";

        private readonly Dictionary<int, string> menu;
        private readonly Dictionary<int, Action> actions;

        private readonly IRepositoryCentral central;
        private readonly IPersonRegistrationService service;

        private bool alive = true;
        private readonly Terminal terminal;

        public Program(IRepositoryCentral central, IPersonRegistrationService service)
        {
            this.service = service;
            this.central = central;
            terminal = new Terminal();

            menu = new Dictionary<int, string>
            {
                [1] = "Register",
                [0] = "Exit",
            };
            actions = new Dictionary<int, Action>
            {
                [1] = Register,
                [0] = Exit,
            };
        }

        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            new ApplicationBundle().Install(services);
            services.AddSingleton<Program>();

            var provider = services.BuildServiceProvider();
            var program = provider.GetRequiredService<Program>();
            program.Run();
        }

        public void Run()
        {
            terminal.Logo("ESCHER TERMINAL");
            while (alive)
            {
                terminal.Title("MAIN MENU");
                var option = terminal.AskMenu(menu);
                var action = actions[option];
                terminal.WriteLine();
                action();
                terminal.WriteLine();
            }
        }

        public void Register()
        {
            terminal.Title(REGISTRATION);

            var person = AskPerson();
            if (person == null) return;

            if (terminal.AskYesNo("Enter details for a spouse?"))
            {
                var spouse = AskPerson(SPOUSE);
                if (spouse == null) return;
                person.AsFamily(spouse);
            }

            try
            {
                service.Register(person);
                central.Commit();
                terminal.Success(REGISTRATION);
            }
            catch (Exception e)
            {
                terminal.Deny(REGISTRATION, e.Message);
            }
        }

        public Person AskPerson(string subject = PERSON)
        {
            terminal.WriteLine();
            terminal.Write("Please provide ");
            terminal.Write(subject.ToUpper(), ConsoleColor.Magenta);
            terminal.Write(" details (* are required fields).");
            terminal.WriteLine();

            var person = new Person
            {
                FirstName = terminal.Ask("First Name"),
                LastName = terminal.Ask("Last Name"),
            };

            var message = AskBirthDate(person, subject);
            if (message != null)
            {
                terminal.Deny(REGISTRATION, message);
                return null;
            }

            return person;
        }

        public string AskBirthDate(Person person, string subject = PERSON)
        {
            person.BirthDate = terminal.Ask("Birth Date", DateTime.Parse);
            var status = service.VerifyAge(person.BirthDate, false);
            return status switch
            {
                PersonVerifyStatus.Unauthorized => AskAuthorization(person, subject),
                _ => service.VerificationMessage(status, subject),
            };
        }

        public string AskAuthorization(Person person, string subject = PERSON)
        {
            person.Authorized = terminal.AskYesNo("Do you have parents authorization?");
            var status = service.VerifyAge(person.BirthDate, person.IsAuthorized);
            return service.VerificationMessage(status, subject);
        }

        public void Exit()
        {
            alive = false;
            terminal.Write("Exiting the terminal...");
        }
    }
}