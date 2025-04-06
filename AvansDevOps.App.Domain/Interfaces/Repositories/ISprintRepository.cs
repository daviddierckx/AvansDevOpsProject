using AvansDevOps.App.Domain.Entities;
using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Interfaces.Repositories
{
    public interface ISprintRepository : IRepository<Sprint>
    {
        // Specifieke methoden voor Sprint indien nodig
        IEnumerable<Sprint> GetSprintsByProject(int projectId);
    }
}