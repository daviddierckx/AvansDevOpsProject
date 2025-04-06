using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Infrastructure.Persistence.FakeRepositories
{
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = new List<User>();
        private int _nextId = 1;

        public void Add(User entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            if (!_users.Any(u => u.Id == entity.Id || u.Email.Equals(entity.Email, StringComparison.OrdinalIgnoreCase)))
            {
                _users.Add(entity);
            }
            // Else: Duplicaat ID of Email, negeer of gooi exception
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _users.Remove(user);
            }
        }

        public void Delete(User entity)
        {
            if (entity != null) Delete(entity.Id);
        }

        public IEnumerable<User> GetAll()
        {
            return _users.ToList();
        }

        public User GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public User GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Developer> GetAllDevelopers()
        {
            return _users.OfType<Developer>().ToList();
        }

        public IEnumerable<ScrumMaster> GetAllScrumMasters()
        {
            return _users.OfType<ScrumMaster>().ToList();
        }

        public IEnumerable<ProductOwner> GetAllProductOwners()
        {
            return _users.OfType<ProductOwner>().ToList();
        }

        public void Update(User entity)
        {
            var existingUser = GetById(entity.Id);
            if (existingUser != null)
            {
                int index = _users.IndexOf(existingUser);
                _users[index] = entity; // Vervang object
            }
        }
    }
}