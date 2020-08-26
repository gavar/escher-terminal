using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Text.Json;
using FlatFile.Core;
using FlatFile.Delimited;

namespace Escher.Data.Engine.Flat
{
    public abstract class FlatFileRepository<T, ID> : IRepository<T>
        where T : class, IPersistable<ID>, new()
    {
        protected FlatFileMeta meta;

        protected readonly string filePath;
        protected readonly string metaPath;

        protected readonly ILineBulder lineBuilder;
        protected readonly IDelimitedLayoutDescriptor descriptor;

        protected readonly IDictionary<ID, T> indexer = new Dictionary<ID, T>();
        protected readonly IDictionary<T, EntityState> states = new Dictionary<T, EntityState>();

        public FlatFileRepository(string? path = null, IDelimitedLayoutDescriptor? descriptor = null)
        {
            this.descriptor = descriptor ?? FlatFile.GetDescriptor(typeof(T));
            filePath = path ?? FlatFile.GetStorePath(this.descriptor);
            lineBuilder = FlatFile.GetBuilder(this.descriptor);

            metaPath = filePath + ".meta";
            using (var stream = Open(metaPath))
                meta = ReadMeta(stream);
        }

        /// <inheritdoc />
        public T Insert(T entity)
        {
            // only update state if already exists
            if (states.ContainsKey(entity))
            {
                states[entity] = EntityState.Added;
                return entity;
            }

            // TODO: better message
            // TODO: EntityExistsException
            if (!entity.IsNew)
                if (indexer.TryGetValue(entity.Id, out var existing))
                    if (entity == existing) return existing;
                    else throw new EntityException("Entity with the same key already exists.");

            states[entity] = EntityState.Added;
            return entity;
        }

        /// <inheritdoc />
        public void Commit()
        {
            using (var inserts = Pool.Rent<T>(states.Count))
            using (Pool.StringBuilders.Rent(out var sb))
            {
                foreach (var (entity, state) in states)
                    switch (state)
                    {
                        // assign identifiers for new entities
                        case EntityState.Added:
                            inserts.Push(entity);
                            var id = EnsurePrimaryKey(entity);
                            indexer.Add(id, entity);
                            break;

                        default:
                            throw new NotSupportedException($"{state} is not supported yet");
                    }

                // accumulating into a temporary buffer to abort transaction if any fails
                for (var i = 0; i < inserts.size; i++)
                    sb.AppendLine(lineBuilder.BuildLine(inserts.array[i]));

                // fast state reset
                states.Clear();

                // flush to the file
                using (var file = Append(filePath))
                using (var writer = new StreamWriter(file))
                {
                    // make sure to write header for a new file
                    if (file.Length == 0 && descriptor.HasHeader)
                        WriteHeader(writer);

                    // flush
                    writer.Write(sb);
                }
            }

            // rewriting meta file with a new state
            using (var stream = Truncate(metaPath))
                WriteMeta(meta, stream);
        }

        /// <summary> Generate incremental key to use for a new entity. </summary>
        protected abstract ID NextKey();

        /// <summary>Automatically generate primary key for the given entity if required. </summary>
        /// <param name="entity"> entity to ensure primary key for.</param>
        /// <returns>Primary key of the provided entity.</returns>
        protected ID EnsurePrimaryKey(T entity)
        {
            if (entity.IsNew)
                return entity.Id = NextKey();

            var id = entity.Id;
            GrowSeed(id);
            return id;
        }

        /// <summary>Advance seed so that given key would never be used as incremental key. </summary>
        /// <param name="key">Primary key explicitly defined by the entity.</param>
        protected abstract void GrowSeed(ID key);

        /// <summary> Writes the header. </summary>
        /// <param name="writer">The writer.</param>
        protected void WriteHeader(TextWriter writer)
        {
            using (Pool.StringBuilders.Rent(out var sb))
            {
                var index = 0;
                foreach (var field in descriptor.Fields)
                {
                    if (index++ > 0) sb.Append(descriptor.Delimiter);
                    sb.Append(field.Name);
                }

                writer.WriteLine(sb);
            }
        }

        private static void WriteMeta(FlatFileMeta meta, Stream stream)
        {
            JsonSerializer.SerializeAsync(stream, meta, FlatFileMeta.SERIALIZER_OPTIONS).Wait();
        }

        private static FlatFileMeta ReadMeta(Stream stream)
        {
            if (stream.Length < 1)
                return new FlatFileMeta();

            var value = JsonSerializer.DeserializeAsync<FlatFileMeta>(stream, FlatFileMeta.SERIALIZER_OPTIONS);
            if (value.IsCompleted) return value.Result;
            var task = value.AsTask();
            task.Wait();

            return task.Result;
        }

        private static FileStream Open(string path)
        {
            return File.Open(path, FileMode.OpenOrCreate);
        }

        private static FileStream Append(string path)
        {
            return File.Open(path, FileMode.Append);
        }

        private static FileStream Truncate(string path)
        {
            return File.Open(path, FileMode.Truncate);
        }
    }
}