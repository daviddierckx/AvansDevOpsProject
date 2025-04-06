using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;
using System.Linq; // Nodig voor .All()

namespace AvansDevOps.App.Domain.States.BacklogItemStates
{
    public class TestedState : IBacklogItemState
    {
        private readonly BacklogItem _backlogItem;

        public TestedState(BacklogItem backlogItem)
        {
            _backlogItem = backlogItem;
        }

        // ... (andere methoden zoals StartTask, MarkAsReadyForTesting, etc. gooien InvalidStateException) ...

        public void StartTask()
        {
            throw new InvalidStateException("Cannot start task: Item has already been tested.");
        }

        public void MarkAsReadyForTesting()
        {
            throw new InvalidStateException("Cannot mark as ready for testing: Item has already been tested.");
        }

        public void StartTesting()
        {
            throw new InvalidStateException("Cannot start testing: Item has already been tested.");
        }

        public void SendTestingResult(bool passed)
        {
            // In Tested state is het resultaat al bekend (true). Opnieuw aanroepen is ongeldig.
            throw new InvalidStateException("Cannot send testing result again: Item has already passed testing.");
        }

        public void CompleteTask()
        {
            // --- CONTROLEER OF ALLE ACTIVITEITEN KLAAR ZIJN ---
            // Deze check is essentieel en moet hier staan.
            if (!_backlogItem.Activities.All(a => a.IsDone()))
            {
                // Gooi hier de InvalidOperationException zoals de test verwacht
                throw new InvalidOperationException($"Cannot complete item '{_backlogItem.Title}': Not all its activities are marked as done.");
            }
            // --- EINDE CHECK ---

            // Alleen als de check slaagt, mag de state veranderen
            _backlogItem.SetState(new DoneState(_backlogItem));
            // Notificatie gebeurt impliciet via SetState -> NotifyObservers
        }

        public void ReopenTask()
        {
            // Vanuit Tested kan het terug naar Todo
            _backlogItem.SetState(new TodoState(_backlogItem));
            // Notificatie gebeurt impliciet via SetState -> NotifyObservers
        }
    }
}
