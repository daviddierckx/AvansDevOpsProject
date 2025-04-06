namespace AvansDevOps.App.Domain.Entities
{
    public class DeployAction : PipelineAction
    {
        public string Environment { get; private set; } // e.g., "Production", "Staging", "Test"
        public string ServerAddress { get; private set; }

        // Simulatie van deploy falen
        private bool _shouldFail;

        public DeployAction(string name, string environment, string serverAddress, bool shouldFail = false) : base(name)
        {
            Environment = environment;
            ServerAddress = serverAddress;
            _shouldFail = shouldFail;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Deploying to {Environment} environment ({ServerAddress})...");
            // Simulatie
            if (_shouldFail)
            {
                Console.WriteLine($"   !!! Deployment to {Environment} FAILED! Check server logs. !!!");
                return false;
            }
            else
            {
                Console.WriteLine($"   Deployment to {Environment} completed successfully.");
                return true;
            }
        }
    }
}