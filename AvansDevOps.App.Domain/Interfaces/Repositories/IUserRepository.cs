using AvansDevOps.App.Domain.Entities;

namespace AvansDevOps.App.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        // Specifieke methoden voor User indien nodig
        User GetByEmail(string email);
        IEnumerable<Developer> GetAllDevelopers();
        IEnumerable<ScrumMaster> GetAllScrumMasters();
        IEnumerable<ProductOwner> GetAllProductOwners();
    }
}