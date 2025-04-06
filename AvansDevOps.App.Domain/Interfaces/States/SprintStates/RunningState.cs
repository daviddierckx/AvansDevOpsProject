using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class RunningState : ISprintState
    {
        private readonly Sprint _sprint;

        public RunningState(Sprint sprint)
        {
            _sprint = sprint;
        }

        // In Running state, backlog items and sprint details cannot be easily changed.
        public void AddBacklogItem(BacklogItem item)
        {
            throw new InvalidStateException("Cannot add backlog items while the sprint is running.");
        }

        public void RemoveBacklogItem(BacklogItem item)
        {
            throw new InvalidStateException("Cannot remove backlog items while the sprint is running.");
        }

        public void ChangeName(string newName)
        {
            throw new InvalidStateException("Cannot change sprint name while the sprint is running.");
        }

        public void ChangeDates(DateTime newStart, DateTime newEnd)
        {
            // End date might be adjustable? Let's restrict for now.
            throw new InvalidStateException("Cannot change sprint dates while the sprint is running.");
        }


        public void StartSprint()
        {
            throw new InvalidStateException("Sprint is already running.");
        }

        public void FinishSprint()
        {
            // Kan alleen finishen na de eind datum? Of mag het eerder als alles af is?
            // Laten we zeggen: alleen na EndDate of als PO/SM het forceert.
            // Vereenvoudiging: Accepteer finish altijd vanuit Running.
            if (DateTime.Now < _sprint.EndDate && !_sprint.SprintBacklog.AllItemsDone()) // Extra check
            {
                Console.WriteLine($"Warning: Finishing sprint '{_sprint.Name}' before end date ({_sprint.EndDate:d}) and not all items are done.");
                // Gooi geen exception, maar geef een waarschuwing. PO/SM beslist.
            }

            _sprint.SetState(new FinishedState(_sprint));
        }

        public void StartRelease(Action<bool> releaseCallback)
        {
            throw new InvalidStateException("Cannot start release: Sprint must be finished first.");
        }

        public void CancelRelease()
        {
            throw new InvalidStateException("Cannot cancel release: No release in progress for a running sprint.");
        }

        public void CloseSprint()
        {
            throw new InvalidStateException("Cannot close sprint: Sprint must be finished first.");
        }

        public void ReviewSprint(string reviewDocumentPath)
        {
            throw new InvalidStateException("Cannot review sprint: Sprint must be finished first.");
        }
    }
}