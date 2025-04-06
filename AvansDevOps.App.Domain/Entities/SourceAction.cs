namespace AvansDevOps.App.Domain.Entities
{
    public class SourceAction : PipelineAction
    {
        public string RepositoryUrl { get; private set; }
        public string Branch { get; private set; }

        public SourceAction(string name, string repositoryUrl, string branch) : base(name)
        {
            RepositoryUrl = repositoryUrl;
            Branch = branch;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Fetching source from {RepositoryUrl} (Branch: {Branch})...");
            // Simulatie: Altijd succesvol
            Console.WriteLine($"   Source fetched successfully.");
            return true;
        }
    }
}