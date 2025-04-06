using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class ReleasedState : ISprintState
    {
        private readonly Sprint _sprint;

        public ReleasedState(Sprint sprint)
        {
            _sprint = sprint;
        }

        // Geen wijzigingen toegestaan na release
        public void AddBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is already released.");
        public void RemoveBacklogItem(BacklogItem item) => throw new InvalidStateException("Sprint is already released.");
        public void ChangeName(string newName) => throw new InvalidStateException("Sprint is already released.");
        public void ChangeDates(DateTime newStart, DateTime newEnd) => throw new InvalidStateException("Sprint is already released.");
        public void StartSprint() => throw new InvalidStateException("Sprint is already released.");
        public void FinishSprint() => throw new InvalidStateException("Sprint is already released.");
        public void StartRelease(Action<bool> callback) => throw new InvalidStateException("Sprint is already released.");
        public void CancelRelease() => throw new InvalidStateException("Cannot cancel release: Sprint has already been released.");


        public void CloseSprint()
        {
            // Dit is de bedoeling na een succesvolle release.
            Console.WriteLine($"Closing successfully released sprint '{_sprint.Name}'.");
            // Eventueel archiveren of andere afrondende acties.
            // SetState naar null of een aparte ClosedState? Voor nu, geen state change meer.
            // Of misschien toch een ClosedState om verdere acties *echt* te blokkeren.
            // Laten we voor nu aannemen dat er geen verdere state is na Close.
            _sprint.NotifyObservers($"Sprint '{_sprint.Name}' has been closed after successful release.");
            // TODO: Implementeer een echte ClosedState als nodig.
        }

        public void ReviewSprint(string docPath) => throw new InvalidStateException("Cannot review sprint: It was a release sprint.");
    }
}