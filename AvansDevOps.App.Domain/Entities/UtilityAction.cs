namespace AvansDevOps.App.Domain.Entities
{
    public class UtilityAction : PipelineAction
    {
        public string Command { get; private set; } // e.g., "copy", "delete", "run script"
        public List<string> Arguments { get; private set; }

        public UtilityAction(string name, string command, List<string> arguments) : base(name)
        {
            Command = command;
            Arguments = arguments;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Executing utility command '{Command}' with arguments: {string.Join(" ", Arguments)}...");
            // Simulatie: Altijd succesvol
            Console.WriteLine($"   Utility command executed successfully.");
            return true;
        }
    }
}