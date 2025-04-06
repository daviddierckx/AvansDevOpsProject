using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Interfaces.Repositories
{
    // Generieke Repository Interface (Basis CRUD)
    public interface IRepository<T> where T : class // Zorg dat T een class is
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Delete(T entity); // Overload
    }
}