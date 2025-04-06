namespace AvansDevOps.App.Domain.Entities
{
    public class BuildAction : PipelineAction
    {
        public string BuildConfiguration { get; private set; } // e.g., "Release", "Debug"
        public string Platform { get; private set; } // e.g., "Any CPU"

        public BuildAction(string name, string configuration = "Release", string platform = "Any CPU") : base(name)
        {
            BuildConfiguration = configuration;
            Platform = platform;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Building project (Config: {BuildConfiguration}, Platform: {Platform})...");
            // Simulatie: Altijd succesvol
            Console.WriteLine($"   Build completed successfully.");
            return true;
        }
    }
}