using AvansDevOps.App.Domain.Entities;
using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Interfaces.Repositories
{
    public interface IBacklogItemRepository : IRepository<BacklogItem>
    {
        // Specifieke methoden voor BacklogItem indien nodig
        IEnumerable<BacklogItem> GetItemsBySprint(int sprintId);
        IEnumerable<BacklogItem> GetItemsWithoutSprint();
    }
}