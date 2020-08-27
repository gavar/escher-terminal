using System.Collections.Generic;

namespace Escher.Data
{
    public interface IRepositoryCentral
    {
        /// <summary> Get all repositories registered to this central. </summary>
        /// <returns></returns>
        IEnumerable<IRepository> GetAll();

        /// <summary> Get repository of the particular type. </summary>
        /// <typeparam name="T">Type of the repository to search for.</typeparam>
        T GetRepository<T>() where T : IRepository;

        /// <summary>
        /// Commit changes for every repository controlled by this central.
        /// </summary>
        public void Commit()
        {
            foreach (var repository in GetAll())
                repository.Commit();
        }
    }
}