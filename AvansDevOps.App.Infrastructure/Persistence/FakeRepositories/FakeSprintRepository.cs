using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Infrastructure.Persistence.FakeRepositories
{
    public class FakeSprintRepository : ISprintRepository
    {
        private readonly List<Sprint> _sprints = new List<Sprint>();
        private int _nextId = 1;

        public void Add(Sprint entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            if (!_sprints.Any(s => s.Id == entity.Id))
            {
                _sprints.Add(entity);
            }
        }

        public void Delete(int id)
        {
            var sprint = GetById(id);
            if (sprint != null)
            {
                _sprints.Remove(sprint);
            }
        }

        public void Delete(Sprint entity)
        {
            if (entity != null) Delete(entity.Id);
        }

        public IEnumerable<Sprint> GetAll()
        {
            return _sprints.ToList();
        }

        public Sprint GetById(int id)
        {
            return _sprints.FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Sprint> GetSprintsByProject(int projectId)
        {
            // Dit vereist een ProjectId op Sprint, wat nu niet direct aanwezig is.
            // We voegen een Project referentie toe aan Sprint of zoeken via ProjectRepo.
            // Aanname: We hebben toegang tot ProjectRepo of Sprint heeft Project referentie.
            // Voor deze FakeRepo simuleren we het, maar dit duidt op een model aanpassing.
            // Laten we aannemen dat Sprint een _project field heeft (zie Sprint.cs aanpassing).
            // return _sprints.Where(s => s._project?.Id == projectId).ToList();
            // Omdat _project private is, is dit lastig.
            // Beter: IProjectRepository.GetById(projectId).Sprints;
            // Voor nu, return alle sprints als placeholder.
            Console.WriteLine($"Warning: GetSprintsByProject in FakeRepo not fully implemented. Returning all sprints.");
            return GetAll();
        }

        public void Update(Sprint entity)
        {
            var existingSprint = GetById(entity.Id);
            if (existingSprint != null)
            {
                int index = _sprints.IndexOf(existingSprint);
                _sprints[index] = entity; // Vervang object
                                          // Let op: State object wordt mee vervangen. Dit is ok voor in-memory.
            }
        }
    }
}