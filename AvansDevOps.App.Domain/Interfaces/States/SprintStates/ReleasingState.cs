using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class ReleasingState : ISprintState
    {
        private readonly Sprint _sprint;
        private readonly Action<bool> _releaseCallback; // Callback om resultaat te melden

        public ReleasingState(Sprint sprint, Action<bool> releaseCallback)
        {
            _sprint = sprint;
            _releaseCallback = releaseCallback; // Store the callback
            Console.WriteLine($"Sprint '{_sprint.Name}' is now releasing...");
            // Hier zou de daadwerkelijke trigger van de pipeline moeten komen.
            // De pipeline executor roept dan _releaseCallback aan.
            // Voor simulatie, doen we dit direct of in Program.cs
        }

        // Geen wijzigingen toegestaan tijdens release
        public void AddBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is currently releasing.");
        public void RemoveBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is currently releasing.");
        public void ChangeName(string newName) => throw new InvalidStateException("Sprint is currently releasing.");
        public void ChangeDates(DateTime newStart, DateTime newEnd) => throw new InvalidStateException("Sprint is currently releasing.");
        public void StartSprint() => throw new InvalidStateException("Sprint is currently releasing.");
        public void FinishSprint() => throw new InvalidStateException("Sprint is currently releasing.");
        public void StartRelease(Action<bool> callback) => throw new InvalidStateException("Sprint release is already in progress.");

        public void CancelRelease()
        {
            // Hier zou je de lopende pipeline moeten proberen te stoppen.
            Console.WriteLine($"Attempting to cancel release for sprint '{_sprint.Name}'...");
            // Na annulering -> Cancelled state
            _sprint.SetState(new CancelledState(_sprint));
            _releaseCallback?.Invoke(false); // Meld dat de release (uiteindelijk) mislukt is door annulering
            _sprint.NotifyObservers($"Release for sprint '{_sprint.Name}' was cancelled by user.");
            // Notify PO and SM specifically (handled by observer logic in User subclasses)
        }

        public void CloseSprint() => throw new InvalidStateException("Cannot close sprint: Release process must complete or be cancelled first.");
        public void ReviewSprint(string docPath) => throw new InvalidStateException("Cannot review sprint during release.");

        // Deze methode wordt *extern* aangeroepen (bv. door PipelineExecutor) om het resultaat te melden
        public void HandleReleaseResult(bool success)
        {
            if (success)
            {
                _sprint.SetState(new ReleasedState(_sprint));
                // Notify PO and SM (Observer pattern handles this via SetState -> NotifyObservers)
            }
            else
            {
                // Casus: Als pipeline faalt -> terug naar Finished state? Of een nieuwe Failed state?
                // Laten we zeggen: terug naar Finished, zodat PO kan beslissen (bv. opnieuw proberen of annuleren).
                // Of direct naar Cancelled? Nee, laten we PO de keus geven.
                _sprint.SetState(new FinishedState(_sprint)); // Terug naar Finished
                _sprint.NotifyObservers($"Pipeline execution failed for sprint '{_sprint.Name}'. Sprint back to Finished state.");
                // Notify PO and SM specifically (handled by observer logic in User subclasses)
            }
            // Roep de originele callback aan die de aanstichter van de release gaf
            _releaseCallback?.Invoke(success);
        }
    }
}