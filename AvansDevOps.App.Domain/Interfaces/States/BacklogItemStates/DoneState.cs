using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    public class DoneState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public DoneState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        // In Done state, most actions are invalid.
        public void StartTask()
        {
            throw new InvalidStateException("Cannot start task: Item is already done.");
        }

        public void MarkAsReadyForTesting()
        {
            throw new InvalidStateException("Cannot mark as ready for testing: Item is already done.");
        }

        public void StartTesting()
        {
            throw new InvalidStateException("Cannot start testing: Item is already done.");
        }

        public void SendTestingResult(bool passed)
        {
            throw new InvalidStateException("Cannot send testing result: Item is already done.");
        }

        public void CompleteTask()
        {
            // --- GOOI HIER DE EXCEPTION ---
            // Console.WriteLine("Item is already marked as 'Done'."); // Oude code?
            throw new InvalidStateException("Cannot complete task: Item is already done.");
            // --- EINDE AANPASSING ---
        }

        public void ReopenTask()
        {
            // Casus: Vanuit Done kan het terug naar Todo (bv. PO keurt het af tijdens review)
            _backlogItem.SetState(new TodoState(_backlogItem));
            // _backlogItem.NotifyObservers($"Item '{_backlogItem.Title}' was reopened and moved back to Todo from Done."); // Notify zit al in SetState
        }
    }
}
