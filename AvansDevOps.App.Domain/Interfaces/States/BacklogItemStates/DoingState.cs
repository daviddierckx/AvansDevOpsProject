using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    // State Pattern: Concrete State
    public class DoingState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public DoingState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        public void StartTask()
        {
            // Kan niet opnieuw gestart worden
            throw new InvalidStateException("Task is already in 'Doing' state.");
        }

        public void MarkAsReadyForTesting()
        {
            _backlogItem.SetState(new ReadyForTestingState(_backlogItem));
            // Notificatie naar testers (Observer Pattern - afgehandeld in SetState/NotifyObservers)
            _backlogItem.NotifyObservers($"Item '{_backlogItem.Title}' is Ready for Testing.");
        }

        public void StartTesting()
        {
            throw new InvalidStateException("Cannot start testing: Task must be marked as 'Ready for Testing' first.");
        }

        public void SendTestingResult(bool passed)
        {
            throw new InvalidStateException("Cannot send testing result: Task is still in 'Doing' state.");
        }

        public void CompleteTask()
        {
            throw new InvalidStateException("Cannot complete task: Task must go through testing first.");
        }

        public void ReopenTask()
        {
            // Regel uit casus: terug naar Todo, niet terug naar Doing.
            _backlogItem.SetState(new TodoState(_backlogItem));
            _backlogItem.NotifyObservers($"Item '{_backlogItem.Title}' moved back to Todo from Doing.");
        }
    }
}