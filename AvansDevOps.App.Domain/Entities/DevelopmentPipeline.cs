using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Domain.Entities
{
    public class DevelopmentPipeline
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PipelineAction> Actions { get; private set; }

        public DevelopmentPipeline(string name)
        {
            Name = name;
            Actions = new List<PipelineAction>();
        }

        public void AddAction(PipelineAction action)
        {
            // Eventueel logica voor volgorde (bv. Source moet eerst)
            Actions.Add(action);
        }

        // Voert de pipeline uit (simulatie)
        public bool Execute()
        {
            Console.WriteLine($"\n========= Starting Pipeline: {Name} =========");
            bool success = true;
            foreach (var action in Actions)
            {
                Console.WriteLine($"--- Executing Action: {action.GetType().Name} ({action.Name}) ---");
                if (!action.Execute()) // Voer de actie uit
                {
                    Console.WriteLine($"!!! Action Failed: {action.GetType().Name} ({action.Name}) !!!");
                    success = false;
                    // Moet de pipeline stoppen bij falen? Ja, meestal wel.
                    break;
                }
                Console.WriteLine($"--- Action Succeeded: {action.GetType().Name} ({action.Name}) ---");
            }
            Console.WriteLine($"========= Pipeline {Name} Finished: {(success ? "SUCCESS" : "FAILED")} =========");
            return success;
        }

        // Helper om te checken of de pipeline eindigt met deployment
        public bool EndsWithDeployment()
        {
            return Actions.LastOrDefault() is DeployAction;
        }
    }
}