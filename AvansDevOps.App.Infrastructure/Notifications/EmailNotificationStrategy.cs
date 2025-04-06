using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using System;

namespace AvansDevOps.App.Infrastructure.Notifications
{
    // Strategy Pattern: Concrete Strategy
    public class EmailNotificationStrategy : INotificationStrategy
    {
        public void Send(string message, User recipient)
        {
            if (string.IsNullOrEmpty(recipient.Email))
            {
                Console.WriteLine($"   - Could not send Email to {recipient.Name}: Email address is missing.");
                return; // Of throw exception?
            }

            // Simulatie van e-mail versturen
            Console.WriteLine($"   - SIMULATING sending Email to: {recipient.Email}");
            Console.WriteLine($"   - Subject: Notification from AvansDevOps");
            Console.WriteLine($"   - Body: Dear {recipient.Name},\n\n{message}\n\nRegards,\nAvans DevOps System");
            Console.WriteLine($"   - Email sent successfully (simulation).");
        }
    }
}