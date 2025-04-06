using AvansDevOps.App.Domain.Entities;

namespace AvansDevOps.App.Domain.Interfaces.Strategies
{
    // Strategy Pattern Interface
    public interface INotificationStrategy
    {
        void Send(string message, User recipient);
    }
}