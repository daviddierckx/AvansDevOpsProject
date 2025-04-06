namespace AvansDevOps.App.Domain.Entities
{
    public class PackageAction : PipelineAction
    {
        public List<string> Packages { get; private set; }

        public PackageAction(string name, List<string> packages) : base(name)
        {
            Packages = packages;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Installing packages: {string.Join(", ", Packages)}...");
            // Simulatie: Altijd succesvol
            Console.WriteLine($"   Packages installed successfully.");
            return true;
        }
    }
}