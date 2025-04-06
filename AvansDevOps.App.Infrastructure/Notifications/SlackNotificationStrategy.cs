using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using System;

namespace AvansDevOps.App.Infrastructure.Notifications
{
    // Strategy Pattern: Concrete Strategy
    public class SlackNotificationStrategy : INotificationStrategy
    {
        public void Send(string message, User recipient)
        {
            if (string.IsNullOrEmpty(recipient.SlackUsername))
            {
                Console.WriteLine($"   - Could not send Slack message to {recipient.Name}: Slack username is missing.");
                return;
            }

            // Simulatie van Slack bericht sturen
            Console.WriteLine($"   - SIMULATING sending Slack message to user: @{recipient.SlackUsername}");
            Console.WriteLine($"   - Message: {message}");
            Console.WriteLine($"   - Slack message sent successfully (simulation).");
        }
    }
}