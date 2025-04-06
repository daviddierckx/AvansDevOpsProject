using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class CancelledState : ISprintState
    {
        private readonly Sprint _sprint;

        public CancelledState(Sprint sprint)
        {
            _sprint = sprint;
        }

        // Geen wijzigingen toegestaan na annulering
        public void AddBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint release was cancelled.");
        public void RemoveBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint release was cancelled.");
        public void ChangeName(string newName) => throw new InvalidStateException("Sprint release was cancelled.");
        public void ChangeDates(DateTime newStart, DateTime newEnd) => throw new InvalidStateException("Sprint release was cancelled.");
        public void StartSprint() => throw new InvalidStateException("Sprint release was cancelled.");
        public void FinishSprint() => throw new InvalidStateException("Sprint release was cancelled.");
        public void StartRelease(Action<bool> callback) => throw new InvalidStateException("Cannot start release: Previous release was cancelled. Reset sprint state first if needed.");
        public void CancelRelease() => Console.WriteLine("Sprint release was already cancelled.");


        public void CloseSprint()
        {
            // Na annulering kan de sprint gesloten worden.
            Console.WriteLine($"Closing sprint '{_sprint.Name}' after release cancellation.");
            // Items blijven mogelijk in een onaffe staat, PO/SM moet beslissen wat ermee gebeurt.
            _sprint.NotifyObservers($"Sprint '{_sprint.Name}' has been closed after release cancellation.");
            // TODO: Implementeer een echte ClosedState als nodig.
        }

        public void ReviewSprint(string docPath) => throw new InvalidStateException("Cannot review sprint: Release was cancelled.");
    }
}