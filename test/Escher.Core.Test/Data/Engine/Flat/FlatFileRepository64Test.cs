using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Runtime.CompilerServices;
using Escher.Xunit;
using FlatFile.Delimited.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Escher.Data.Engine.Flat
{
    public class FlatFileRepository64Test
    {
        private readonly ITestOutputHelper output;

        public FlatFileRepository64Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void PrimaryKeyAssigned()
        {
            var filePath = SetupFilePath();
            using (var repository = new FlatFileRepository64<Basic>(filePath))
            {
                var entity = new Basic { FirstName = "John", LastName = "Doe" };
                entity = repository.Insert(entity);
                Assert.Equal(default, entity.Id);
                repository.Commit();
                Assert.Equal(1L, entity.Id);
            }
        }

        [Fact]
        public void CommitToFile()
        {
            var filePath = SetupFilePath();
            var metaPath = filePath + ".meta";

            using (var repository = new FlatFileRepository64<Basic>(filePath))
            {
                Insert(repository, Samples());
                repository.Commit();
            }

            Asserts.FileJson(metaPath, new FlatFileMeta { Seed = 2 });
            Asserts.FileLines(filePath, new[]
            {
                Basic.HEADER,
                "1, John, Doe",
                "2, Janet, Doe",
            });
        }

        [Fact]
        public void AppendToFile()
        {
            var filePath = SetupFilePath();
            var metaPath = filePath + ".meta";

            using (var repository = new FlatFileRepository64<Basic>(filePath))
            {
                Insert(repository, Samples());
                repository.Commit();
                Insert(repository, Samples());
                repository.Commit();
            }

            Asserts.FileJson(metaPath, new FlatFileMeta { Seed = 4 });
            Asserts.FileLines(filePath, new[]
            {
                Basic.HEADER,
                "1, John, Doe",
                "2, Janet, Doe",
                "3, John, Doe",
                "4, Janet, Doe",
            });
        }

        [Fact]
        public void AllowsToAddSameObject()
        {
            var filePath = SetupFilePath();
            var metaPath = filePath + ".meta";

            using (var repository = new FlatFileRepository64<Basic>(filePath))
            {
                var entity = new Basic { FirstName = "John", LastName = "Doe" };
                repository.Insert(entity);
                repository.Commit();
                repository.Insert(entity);
                repository.Commit();
            }

            Asserts.FileJson(metaPath, new FlatFileMeta { Seed = 1 });
            Asserts.FileLines(filePath, new[]
            {
                Basic.HEADER,
                "1, John, Doe",
            });
        }

        [Fact]
        public void ThrowsWhenKeyAlreadyExists()
        {
            var filePath = SetupFilePath();
            using (var repository = new FlatFileRepository64<Basic>(filePath))
            {
                repository.Insert(new Basic { Id = 1 });
                repository.Commit();
                Assert.Throws<EntityException>(() => repository.Insert(new Basic { Id = 1 }));
            }
        }

        [Fact]
        public void WriteForeignKey()
        {
            var filePath = SetupFilePath();
            var metaPath = filePath + ".meta";

            using (var repository = new FlatFileRepository64<Relation>(filePath))
            {
                var a = new Relation { FirstName = "John", LastName = "Doe" };
                var b = new Relation { FirstName = "Janet", LastName = "Doe" };
                a.RelatesTo = b;
                b.RelatesTo = a;

                repository.Insert(a);
                repository.Insert(b);
                repository.Commit();
            }

            Asserts.FileJson(metaPath, new FlatFileMeta { Seed = 2 });
            Asserts.FileLines(filePath, new[]
            {
                Relation.HEADER,
                "1, John, Doe, 2",
                "2, Janet, Doe, 1",
            });
        }

        [Fact]
        public void ThrowsWhenTransientRelation()
        {
            var filePath = SetupFilePath();
            using (var repository = new FlatFileRepository64<Relation>(filePath))
            {
                var b = new Relation { FirstName = "Janet", LastName = "Doe" };
                var a = new Relation { FirstName = "John", LastName = "Doe", RelatesTo = b };
                repository.Insert(a);
                Assert.Throws<EntityException>(() => repository.Commit());
            }
        }

        private string SetupFilePath([CallerMemberName] string caller = "")
        {
            const string directory = "test/" + nameof(FlatFileRepository64Test);
            Directory.CreateDirectory(directory);

            var filePath = directory + "/" + caller + ".csv";
            output.WriteLine(Path.GetFullPath(filePath));

            File.Delete(filePath);
            File.Delete(filePath + ".meta");
            return filePath;
        }

        private static void Insert<T>(IRepository<T> repository, IEnumerable<T> items)
        {
            foreach (var item in items)
                repository.Insert(item);
        }

        public static Basic[] Samples()
        {
            return new[]
            {
                new Basic { FirstName = "John", LastName = "Doe" },
                new Basic { FirstName = "Janet", LastName = "Doe" },
            };
        }

        [DelimitedFile(Delimiter = ", ", HasHeader = true)]
        public class Basic : Entity
        {
            public static readonly string HEADER = string.Join(", ",
                nameof(Id),
                nameof(FirstName),
                nameof(LastName)
            );

            [DelimitedField(2, Name = nameof(FirstName))]
            public string FirstName { get; set; }

            [DelimitedField(3, Name = nameof(LastName))]
            public string LastName { get; set; }
        }

        [DelimitedFile(Delimiter = ", ", HasHeader = true)]
        public class Relation : Basic
        {
            public static readonly string HEADER = string.Join(", ",
                nameof(Id),
                nameof(FirstName),
                nameof(LastName),
                nameof(RelatesTo)
            );

            [DelimitedField(4, Name = nameof(RelatesTo))]
            public Relation RelatesTo { get; set; }
        }
    }
}