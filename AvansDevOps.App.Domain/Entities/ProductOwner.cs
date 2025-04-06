namespace AvansDevOps.App.Domain.Entities
{
    public class ProductOwner : User
    {
        public ProductOwner(string name, string email, string slackUsername = null)
            : base(name, email, slackUsername)
        {
        }

        // Override Update om specifiek gedrag te implementeren indien nodig
        public override void Update(Interfaces.Patterns.ISubject subject, string message)
        {
            base.Update(subject, message); // Roep basis implementatie aan (loggen)

            // Specifieke acties voor Product Owner notificaties
            if (subject is Sprint sprint)
            {
                if (message.Contains("Release Cancelled"))
                {
                    Console.WriteLine($" --> Product Owner {Name} notified: Release for sprint '{sprint.Name}' was CANCELLED!");
                }
                if (message.Contains("Sprint Released"))
                {
                    Console.WriteLine($" --> Product Owner {Name} notified: Sprint '{sprint.Name}' successfully RELEASED.");
                }
            }
        }
    }
}