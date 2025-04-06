namespace AvansDevOps.App.Domain.Entities
{
    public class AnalyseAction : PipelineAction
    {
        public string Tool { get; private set; } // e.g., "SonarQube"
        public string SettingsFile { get; private set; } // Optional settings

        public AnalyseAction(string name, string tool = "SonarQube", string settingsFile = null) : base(name)
        {
            Tool = tool;
            SettingsFile = settingsFile;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Preparing analysis with {Tool}...");
            Console.WriteLine($"   Executing static code analysis...");
            // Simulatie: Altijd succesvol
            Console.WriteLine($"   Analysis completed. Reporting results...");
            return true;
        }
    }
}