using AvansDevOps.App.Domain.Entities;

namespace AvansDevOps.App.Domain.Interfaces.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        // Specifieke methoden voor Project indien nodig
        Project GetByName(string name);
    }
}