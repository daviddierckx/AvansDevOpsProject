namespace AvansDevOps.App.Domain.Interfaces.Patterns
{
    // Composite Pattern: Component Interface
    // Zowel BacklogItem (Composite) als Activity (Leaf) implementeren dit.
    public interface IWorkItem
    {
        bool IsDone();
        void Display(int indentLevel = 0); // Voor het tonen van de hiërarchie
    }
}