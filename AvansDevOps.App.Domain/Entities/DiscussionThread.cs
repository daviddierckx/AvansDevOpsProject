using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Entities
{
    public class DiscussionThread
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public List<Message> Messages { get; private set; }
        public BacklogItem RelatedBacklogItem { get; private set; } // Koppeling

        public DiscussionThread(string subject, BacklogItem relatedItem)
        {
            Subject = subject;
            RelatedBacklogItem = relatedItem;
            Messages = new List<Message>();
        }

        public void AddMessage(Message message)
        {
            // Check if related backlog item is done (Rule from case)
            // We need to check the *actual* state, not just if it *was* done once.
            if (RelatedBacklogItem.CurrentState is States.BacklogItemStates.DoneState)
            {
                throw new InvalidOperationException("Cannot add messages to a discussion for a completed backlog item.");
            }

            Messages.Add(message);
            // Notify relevant users (e.g., thread participants, maybe item assignee?)
            NotifyParticipants(message);
        }

        private void NotifyParticipants(Message newMessage)
        {
            Console.WriteLine($"--- Discussion Notification for Thread '{Subject}' ---");
            var participants = new HashSet<User>(Messages.Select(m => m.Author)); // Get unique authors
            participants.Add(newMessage.Author); // Add the new author

            // Notify backlog item assignee as well? Potentially...
            if (RelatedBacklogItem.AssignedDeveloper != null)
            {
                participants.Add(RelatedBacklogItem.AssignedDeveloper);
            }
            // Notify Scrum Master? Product Owner? Depends on rules. Keep it simple for now.

            foreach (var user in participants)
            {
                if (user != newMessage.Author) // Don't notify the author of their own message
                {
                    // Simplistic notification - ideally uses NotificationService
                    Console.WriteLine($" -> Notifying {user.Name} about new message from {newMessage.Author.Name}: '{newMessage.Content.Substring(0, Math.Min(newMessage.Content.Length, 20))}...'");
                    // user.Update(this, $"New message in '{Subject}' by {newMessage.Author.Name}"); // This might be too noisy
                }
            }
            Console.WriteLine($"--- End Discussion Notification ---");
        }
    }
}