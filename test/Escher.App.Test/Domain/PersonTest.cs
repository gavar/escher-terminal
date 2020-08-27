using System;
using System.IO;
using System.Runtime.CompilerServices;
using Escher.Data;
using Escher.Data.Engine.Flat;
using Escher.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Escher.App.Domain
{
    public class PersonTest
    {
        private readonly ITestOutputHelper output;

        public PersonTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void CheckFileStructure()
        {
            var filePath = SetupFilePath();
            using (var repository = new FlatFileRepository64<Person>(filePath))
            {
                Family(repository, new Person
                {
                    FirstName = "John",
                    LastName = "Doe",
                    BirthDate = new DateTime(1991, 09, 13),
                }, new Person
                {
                    FirstName = "Janet",
                    LastName = "Doe",
                    Authorized = true,
                    BirthDate = new DateTime(2004, 06, 03),
                });
                repository.Commit();
            }

            Asserts.FileLines(filePath, new[]
            {
                "Id, FirstName, LastName, BirthDate, Authorized, Spouse",
                "1, John, Doe, 13.09.1991, , 2",
                "2, Janet, Doe, 03.06.2004, True, 1",
            });
        }

        private void Family(IRepository<Person> repository, Person a, Person b)
        {
            a.Spouse = b;
            b.Spouse = a;
            repository.Insert(a);
            repository.Insert(b);
        }

        private string SetupFilePath([CallerMemberName] string caller = "")
        {
            const string directory = "test/" + nameof(PersonTest);
            Directory.CreateDirectory(directory);

            var filePath = directory + "/" + caller + ".csv";
            output.WriteLine(Path.GetFullPath(filePath));

            File.Delete(filePath);
            File.Delete(filePath + ".meta");
            return filePath;
        }
    }
}