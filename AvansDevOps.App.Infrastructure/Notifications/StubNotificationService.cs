using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Services;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Infrastructure.Notifications
{
    // Stub Implementatie - stuurt geen echte notificaties, logt alleen.
    public class StubNotificationService : INotificationService
    {
        // Injecteer beschikbare strategieën (of vind ze dynamisch)
        private readonly IEnumerable<INotificationStrategy> _availableStrategies;

        public StubNotificationService(IEnumerable<INotificationStrategy> availableStrategies)
        {
            _availableStrategies = availableStrategies;
        }

        public void SendNotification(string message, User recipient)
        {
            Console.WriteLine($"--- Sending Notification to {recipient.Name} ---");
            Console.WriteLine($"   Message: {message}");

            if (!recipient.NotificationPreferences.Any())
            {
                Console.WriteLine($"   WARNING: Recipient {recipient.Name} has no notification preferences set. Notification might not be sent.");
                // Stuur default (bv email) of doe niks? Doe niks voor nu.
                return;
            }

            Console.WriteLine($"   Preferred Channels: {string.Join(", ", recipient.NotificationPreferences.Select(s => s.GetType().Name))}");

            foreach (var strategy in recipient.NotificationPreferences)
            {
                try
                {
                    strategy.Send(message, recipient); // Gebruik de strategie
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ERROR sending notification via {strategy.GetType().Name} to {recipient.Name}: {ex.Message}");
                }
            }
            Console.WriteLine($"--- End Notification for {recipient.Name} ---");
        }

        public void SendNotificationToMultiple(string message, IEnumerable<User> recipients)
        {
            Console.WriteLine($"--- Sending Notification to Multiple Recipients ---");
            Console.WriteLine($"   Message: {message}");
            foreach (var recipient in recipients)
            {
                // Roep de enkele send aan voor de logica per user
                SendNotification(message, recipient);
            }
            Console.WriteLine($"--- End Notification to Multiple ---");
        }

        public void SendNotificationViaStrategy(string message, User recipient, INotificationStrategy strategy)
        {
            Console.WriteLine($"--- Sending Specific Notification to {recipient.Name} via {strategy.GetType().Name} ---");
            Console.WriteLine($"   Message: {message}");
            try
            {
                strategy.Send(message, recipient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ERROR sending notification via {strategy.GetType().Name} to {recipient.Name}: {ex.Message}");
            }
            Console.WriteLine($"--- End Specific Notification for {recipient.Name} ---");
        }
    }
}