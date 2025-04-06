using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    // State Pattern: Concrete State
    public class TestingState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public TestingState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        public void StartTask()
        {
            throw new InvalidStateException("Cannot start task: Item is currently being tested.");
        }

        public void MarkAsReadyForTesting()
        {
            // Could potentially go back? Let's say no for simplicity.
            throw new InvalidStateException("Cannot mark as ready for testing: Item is already being tested.");
        }

        public void StartTesting()
        {
            Console.WriteLine("Item is already being tested.");
        }

        public void SendTestingResult(bool passed)
        {
            if (passed)
            {
                _backlogItem.SetState(new TestedState(_backlogItem));
                _backlogItem.NotifyObservers($"Testing PASSED for item '{_backlogItem.Title}'.");
            }
            else
            {
                // Casus: Als test faalt -> terug naar Todo
                _backlogItem.SetState(new TodoState(_backlogItem));
                // Notificatie naar developer & SM (Observer Pattern - handled by NotifyObservers)
                _backlogItem.NotifyObservers($"Testing FAILED for item '{_backlogItem.Title}'. Moved back to Todo.");
            }
        }

        public void CompleteTask()
        {
            throw new InvalidStateException("Cannot complete task: Testing results must be submitted first.");
        }

        public void ReopenTask()
        {
            // Kan een tester een item terugzetten naar Todo vanuit Testing? Laten we zeggen ja.
            _backlogItem.SetState(new TodoState(_backlogItem));
            _backlogItem.NotifyObservers($"Item '{_backlogItem.Title}' moved back to Todo from Testing by tester action.");
        }
    }
}