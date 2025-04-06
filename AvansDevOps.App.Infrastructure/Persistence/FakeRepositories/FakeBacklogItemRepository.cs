using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Infrastructure.Persistence.FakeRepositories
{
    public class FakeBacklogItemRepository : IBacklogItemRepository
    {
        private readonly List<BacklogItem> _items = new List<BacklogItem>();
        private int _nextId = 1;
        // Referentie nodig naar sprints om te filteren
        private readonly ISprintRepository _sprintRepository;

        public FakeBacklogItemRepository(ISprintRepository sprintRepository)
        {
            _sprintRepository = sprintRepository;
        }

        public void Add(BacklogItem entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = _nextId++;
            }
            if (!_items.Any(i => i.Id == entity.Id))
            {
                _items.Add(entity);
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public void Delete(BacklogItem entity)
        {
            if (entity != null) Delete(entity.Id);
        }


        public IEnumerable<BacklogItem> GetAll()
        {
            return _items.ToList();
        }

        public BacklogItem GetById(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<BacklogItem> GetItemsBySprint(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint != null)
            {
                // Return items die in de sprint backlog van deze specifieke sprint zitten
                // We moeten de IDs matchen met onze _items lijst.
                var sprintItemIds = sprint.SprintBacklog.Items.Select(i => i.Id).ToList();
                return _items.Where(i => sprintItemIds.Contains(i.Id)).ToList();
            }
            return Enumerable.Empty<BacklogItem>();
        }

        public IEnumerable<BacklogItem> GetItemsWithoutSprint()
        {
            var allSprintItemIds = _sprintRepository.GetAll()
                .SelectMany(s => s.SprintBacklog.Items)
                .Select(i => i.Id)
                .Distinct()
                .ToList();

            return _items.Where(i => !allSprintItemIds.Contains(i.Id)).ToList();
        }

        public void Update(BacklogItem entity)
        {
            var existingItem = GetById(entity.Id);
            if (existingItem != null)
            {
                int index = _items.IndexOf(existingItem);
                _items[index] = entity; // Vervang object
            }
        }
    }
}