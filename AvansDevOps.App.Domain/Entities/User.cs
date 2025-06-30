using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.Interfaces.Strategies; // Voor notificatie strategie
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace AvansDevOps.App.Domain.Entities
{
    // User is een IObserver voor het Observer Pattern
    public abstract class User : IObserver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } // Voor notificaties
        public string SlackUsername { get; set; } // Voor notificaties

        // Strategy Pattern: Elke gebruiker kan meerdere manieren hebben om notificaties te ontvangen
        public List<INotificationStrategy> NotificationPreferences { get; private set; }

        protected User(string name, string email, string slackUsername = null)
        {
            Name = name;
            Email = email;
            SlackUsername = slackUsername;
            NotificationPreferences = new List<INotificationStrategy>();
        }

        public void AddNotificationPreference(INotificationStrategy strategy) // Strategy Pattern
        {
            if (!NotificationPreferences.Contains(strategy))
            {
                NotificationPreferences.Add(strategy);
            }
        }

        public void RemoveNotificationPreference(INotificationStrategy strategy) // Strategy Pattern
        {
            NotificationPreferences.Remove(strategy);
        }


        // --- Observer Pattern Implementation ---
        public virtual void Update(ISubject subject, string message)
        {
            Console.WriteLine($"User {Name} received an update from {subject.GetType().Name}: '{message}'");
            // Stuur notificatie via de gekozen strategieën (Strategy Pattern)
            // Dit wordt meestal afgehandeld door een NotificationService die de voorkeuren leest.
            // Hier simuleren we het direct aanroepen voor de eenvoud.
            Console.WriteLine($" -> Preparing to notify {Name} via preferred channels...");
        }
        // --- End Observer Pattern ---

        public override string ToString()
        {
            return $"{GetType().Name}: {Name}";
        }
    }
}