using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class ReviewedState : ISprintState
    {
        private readonly Sprint _sprint;

        public ReviewedState(Sprint sprint)
        {
            _sprint = sprint;
        }

        // Geen wijzigingen toegestaan na review
        public void AddBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is already reviewed.");
        public void RemoveBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is already reviewed.");
        public void ChangeName(string newName) => throw new InvalidStateException("Sprint is already reviewed.");
        public void ChangeDates(DateTime newStart, DateTime newEnd) => throw new InvalidStateException("Sprint is already reviewed.");
        public void StartSprint() => throw new InvalidStateException("Sprint is already reviewed.");
        public void FinishSprint() => throw new InvalidStateException("Sprint is already reviewed.");
        public void StartRelease(Action<bool> callback) => throw new InvalidStateException("Cannot start release: Sprint was a review sprint.");
        public void CancelRelease() => throw new InvalidStateException("Cannot cancel release: Sprint was a review sprint.");


        public void CloseSprint()
        {
            // Na review kan de sprint gesloten worden.
            Console.WriteLine($"Closing reviewed sprint '{_sprint.Name}'.");
            _sprint.NotifyObservers($"Sprint '{_sprint.Name}' has been closed after review.");
            // TODO: Implementeer een echte ClosedState als nodig.
        }

        public void ReviewSprint(string docPath) => Console.WriteLine("Sprint has already been reviewed.");
    }
}