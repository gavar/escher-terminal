using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Escher.Data
{
    public class RepositoryServiceCentral : IRepositoryCentral
    {
        private readonly IServiceProvider services;
        private readonly HashSet<IRepository> repositories;

        public RepositoryServiceCentral(IServiceProvider services)
        {
            this.services = services;
            repositories = new HashSet<IRepository>();
        }

        public IEnumerable<IRepository> GetAll()
        {
            return repositories;
        }

        public T GetRepository<T>() where T : IRepository
        {
            var repository = services.GetRequiredService<T>();
            repositories.Add(repository);
            return repository;
        }
    }
}