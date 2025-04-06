using AvansDevOps.App.Domain.Interfaces.Patterns;

namespace AvansDevOps.App.Domain.Entities
{
    // Activity is een IWorkItem voor het Composite Pattern
    public class Activity : IWorkItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; } // Simpele status voor activiteiten
        public Developer AssignedDeveloper { get; set; } // Optioneel

        public Activity(string description)
        {
            Description = description;
            Completed = false;
        }

        public bool IsDone() // Composite Pattern
        {
            return Completed;
        }

        public void MarkAsDone()
        {
            Completed = true;
            // Potential notification?
        }

        // --- Composite Pattern Implementation ---
        public void Display(int indentLevel = 0)
        {
            Console.WriteLine($"{new string(' ', indentLevel * 2)}- Activity: {Description} [{(Completed ? "Done" : "Open")}] {(AssignedDeveloper != null ? $"(Assigned: {AssignedDeveloper.Name})" : "")}");
        }
        // --- End Composite Pattern ---
    }
}