using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    // State Pattern: Concrete State
    public class ReadyForTestingState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public ReadyForTestingState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        public void StartTask()
        {
            // Kan niet opnieuw gestart worden in deze staat
            throw new InvalidStateException("Cannot start task: Item is ready for testing.");
        }

        public void MarkAsReadyForTesting()
        {
            // Al in deze state, geen actie of melding.
            Console.WriteLine($"Item '{_backlogItem.Title}' is already marked as 'Ready for Testing'.");
        }

        public void StartTesting()
        {
            // Transitie naar de Testing state
            _backlogItem.SetState(new TestingState(_backlogItem));
            _backlogItem.NotifyObservers($"Testing started for item '{_backlogItem.Title}'.");
        }

        public void SendTestingResult(bool passed)
        {
            // Testen moet eerst gestart worden voordat er een resultaat kan zijn.
            throw new InvalidStateException("Cannot send testing result: Testing must be started first (item is currently in 'Ready for Testing' state).");
        }

        public void CompleteTask()
        {
            // Item moet eerst getest worden.
            throw new InvalidStateException("Cannot complete task: Item needs to be tested first (currently in 'Ready for Testing' state).");
        }

        public void ReopenTask()
        {
            // Casus: Als een item in ReadyForTesting wordt afgekeurd (bv. door PO, of omdat het toch niet klaar was),
            // gaat het terug naar Todo.
            _backlogItem.SetState(new TodoState(_backlogItem));
            _backlogItem.NotifyObservers($"Item '{_backlogItem.Title}' moved back to Todo from ReadyForTesting.");
        }
    }
}