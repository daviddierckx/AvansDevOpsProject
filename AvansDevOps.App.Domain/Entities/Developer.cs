namespace AvansDevOps.App.Domain.Entities
{
    public class Developer : User
    {
        public Developer(string name, string email, string slackUsername = null)
            : base(name, email, slackUsername)
        {
        }

        // Specifieke developer methoden indien nodig
    }
}