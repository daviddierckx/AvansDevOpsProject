using AvansDevOps.App.Domain.Interfaces.Patterns;
using System.Text;

namespace AvansDevOps.App.Domain.Entities
{
    // Basis Report class - Concrete Component for Decorator Pattern
    public class Report : IReportComponent
    {
        public string Title { get; set; }
        public Sprint RelatedSprint { get; set; } // Data source

        public Report(string title, Sprint sprint)
        {
            Title = title;
            RelatedSprint = sprint;
        }

        // Decorator Pattern: Basis operatie die gedecoreerd kan worden
        public virtual string GenerateContent()
        {
            var content = new StringBuilder();
            content.AppendLine($"--- Report: {Title} ---");
            content.AppendLine($"Sprint: {RelatedSprint.Name} ({RelatedSprint.StartDate:d} - {RelatedSprint.EndDate:d})");
            content.AppendLine($"Scrum Master: {RelatedSprint.ScrumMaster.Name}");
            content.AppendLine($"Team Members: {string.Join(", ", RelatedSprint.TeamMembers.Select(m => m.Name))}");
            content.AppendLine($"Status: {RelatedSprint.CurrentState.GetType().Name}");
            content.AppendLine("\nBacklog Items:");
            if (RelatedSprint.SprintBacklog.Items.Any())
            {
                foreach (var item in RelatedSprint.SprintBacklog.Items)
                {
                    item.Display(1); // Use composite display
                }
            }
            else
            {
                content.AppendLine("  (No items in sprint backlog)");
            }

            // Add Burndown chart simulation/data placeholder
            content.AppendLine("\nBurndown Chart Data: (Placeholder)");
            // Add effort points per dev placeholder
            content.AppendLine("Effort Points per Developer: (Placeholder)");


            content.AppendLine($"--- End of Report: {Title} ---");
            return content.ToString();
        }
    }
}