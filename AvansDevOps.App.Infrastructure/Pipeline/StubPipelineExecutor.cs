using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.States.SprintStates; // Nodig voor ReleasingState
using AvansDevOps.App.Domain.Exceptions; // Nodig voor InvalidStateException
using System;
using System.Threading.Tasks; // Voor async simulatie

namespace AvansDevOps.App.Infrastructure.Pipeline
{
    // Stub Implementatie - voert geen echte pipeline uit, simuleert alleen.
    public class StubPipelineExecutor
    {
        // Deze methode simuleert het asynchroon uitvoeren van de pipeline
        // en roept de HandleReleaseResult op de ReleasingState aan wanneer klaar.
        public async Task ExecutePipelineAsync(DevelopmentPipeline pipeline, Sprint sprintContext)
        {
            if (pipeline == null)
            {
                Console.WriteLine("PipelineExecutor Error: Pipeline cannot be null.");
                // Hoe melden we dit terug? Via sprint state? Exception?
                // Voor nu, loggen we het. De sprint blijft in de state waarin hij was.
                return;
            }

            // Check of de sprint wel in Releasing state is
            if (!(sprintContext.CurrentState is ReleasingState releasingState))
            {
                Console.WriteLine($"PipelineExecutor Warning: Sprint '{sprintContext.Name}' is not in Releasing state. Current state: {sprintContext.CurrentState.GetType().Name}. Aborting pipeline execution.");
                // Moet hier de release automatisch geannuleerd worden? Nee, dat doet de SprintManager/User.
                return;
            }


            Console.WriteLine($"~~~ Pipeline Executor: Starting execution of '{pipeline.Name}' for sprint '{sprintContext.Name}' (Async Simulation) ~~~");

            // Simuleer wachttijd
            await Task.Delay(TimeSpan.FromSeconds(3)); // Wacht 3 seconden

            // Voer de pipeline acties uit (gesimuleerd)
            bool success = pipeline.Execute(); // De Execute methode van DevelopmentPipeline logt de stappen

            // Simuleer nog wat wachttijd na uitvoering (bv. cleanup)
            await Task.Delay(TimeSpan.FromSeconds(1));

            Console.WriteLine($"~~~ Pipeline Executor: Execution finished for '{pipeline.Name}'. Success: {success}. Notifying sprint state... ~~~");

            // Roep de HandleReleaseResult aan op de *huidige* state van de sprint.
            // Het is belangrijk dat de sprint nog steeds in ReleasingState is.
            // Als de gebruiker tussendoor CancelRelease heeft aangeroepen, staat de sprint
            // al in CancelledState en doet HandleReleaseResult niks meer (of geeft een fout).
            if (sprintContext.CurrentState is ReleasingState currentReleasingState)
            {
                currentReleasingState.HandleReleaseResult(success);
            }
            else
            {
                Console.WriteLine($"PipelineExecutor Warning: Sprint '{sprintContext.Name}' is no longer in Releasing state (Current: {sprintContext.CurrentState.GetType().Name}) when pipeline finished. Result '{success}' ignored by state handler.");
                // De callback in SprintManager wordt mogelijk nog wel aangeroepen, maar de state change is al gebeurd (bv naar Cancelled).
                // De callback moet hier rekening mee houden.
            }

            Console.WriteLine($"~~~ Pipeline Executor: Notification sent to sprint state. Async task complete. ~~~");
        }
    }
}