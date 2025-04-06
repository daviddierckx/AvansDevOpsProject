using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Infrastructure.Persistence.FakeRepositories
{
    // Fake Repository voor in-memory data (Vervangt echte DB interactie)
    public class FakeProjectRepository : IProjectRepository
    {
        private readonly List<Project> _projects = new List<Project>();
        private int _nextId = 1;

        public void Add(Project entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            if (!_projects.Any(p => p.Id == entity.Id))
            {
                _projects.Add(entity);
            }
            // Else: update? Of exception gooien? Voor nu: negeren als ID al bestaat.
        }

        public void Delete(int id)
        {
            var project = GetById(id);
            if (project != null)
            {
                _projects.Remove(project);
            }
        }

        public void Delete(Project entity)
        {
            if (entity != null)
            {
                Delete(entity.Id);
            }
        }

        public IEnumerable<Project> GetAll()
        {
            return _projects.ToList(); // Return een kopie
        }

        public Project GetById(int id)
        {
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        public Project GetByName(string name)
        {
            return _projects.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void Update(Project entity)
        {
            var existingProject = GetById(entity.Id);
            if (existingProject != null)
            {
                // Simpele update: vervang het hele object (of update properties)
                int index = _projects.IndexOf(existingProject);
                _projects[index] = entity;

                // In een echte ORM zou je specifieke velden updaten.
                // existingProject.Name = entity.Name;
                // existingProject.ProductOwner = entity.ProductOwner;
                // ... (Let op met collecties!)
            }
            // Else: wat te doen als het niet bestaat? Exception? Of toevoegen?
            // Voor nu: negeren.
        }
    }
}