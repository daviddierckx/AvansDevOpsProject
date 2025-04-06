using AvansDevOps.App.Domain.Entities;
using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        // Stuurt een bericht naar een specifieke gebruiker via hun voorkeurskanalen
        void SendNotification(string message, User recipient);

        // Stuurt een bericht naar meerdere gebruikers
        void SendNotificationToMultiple(string message, IEnumerable<User> recipients);

        // Stuurt een bericht naar een specifieke gebruiker via een specifiek kanaal (overruled voorkeur)
        void SendNotificationViaStrategy(string message, User recipient, Interfaces.Strategies.INotificationStrategy strategy);
    }
}