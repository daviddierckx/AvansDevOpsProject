using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    // State Pattern: Concrete State
    public class TodoState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public TodoState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        public void StartTask()
        {
            // Kan alleen als er een developer is toegewezen
            if (_backlogItem.AssignedDeveloper == null)
            {
                throw new InvalidStateException("Cannot start task: No developer assigned to the backlog item.");
            }
            _backlogItem.SetState(new DoingState(_backlogItem));
        }

        public void MarkAsReadyForTesting()
        {
            throw new InvalidStateException("Cannot mark as ready for testing: Task must be in 'Doing' state first.");
        }

        public void StartTesting()
        {
            throw new InvalidStateException("Cannot start testing: Task is still in 'Todo' state.");
        }

        public void SendTestingResult(bool passed)
        {
            throw new InvalidStateException("Cannot send testing result: Task is still in 'Todo' state.");
        }

        public void CompleteTask()
        {
            throw new InvalidStateException("Cannot complete task: Task must go through testing first.");
        }

        public void ReopenTask()
        {
            // Is al in Todo, geen actie nodig of foutmelding? Geen actie.
            Console.WriteLine("Task is already in 'Todo' state.");
        }
    }
}