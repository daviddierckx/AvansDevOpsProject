namespace AvansDevOps.App.Domain.Entities
{
    public class ScrumMaster : User
    {
        public ScrumMaster(string name, string email, string slackUsername = null)
            : base(name, email, slackUsername)
        {
        }

        // Override Update om specifiek gedrag te implementeren indien nodig
        public override void Update(Interfaces.Patterns.ISubject subject, string message)
        {
            base.Update(subject, message); // Roep basis implementatie aan (loggen)

            // Specifieke acties voor Scrum Master notificaties
            if (subject is BacklogItem item)
            {
                // Bijv. reageren op "Item Rejected by Tester"
                if (message.Contains("rejected by tester"))
                {
                    Console.WriteLine($" --> Scrum Master {Name} takes note: Speak to developer about rejected item '{item.Title}'.");
                }
            }
            else if (subject is Sprint sprint)
            {
                // Bijv. reageren op "Pipeline Failed"
                if (message.Contains("Pipeline execution failed"))
                {
                    Console.WriteLine($" --> Scrum Master {Name} notified: Pipeline failed for sprint '{sprint.Name}'. Needs investigation.");
                    // Hier zou de SM actie kunnen ondernemen, bv. de pipeline opnieuw starten
                    // of annuleren via de SprintManager/Sprint zelf.
                }
                if (message.Contains("Release Cancelled"))
                {
                    Console.WriteLine($" --> Scrum Master {Name} notified: Release for sprint '{sprint.Name}' was cancelled.");
                }
                if (message.Contains("Sprint Released"))
                {
                    Console.WriteLine($" --> Scrum Master {Name} notified: Sprint '{sprint.Name}' successfully released.");
                }
            }
        }
    }
}