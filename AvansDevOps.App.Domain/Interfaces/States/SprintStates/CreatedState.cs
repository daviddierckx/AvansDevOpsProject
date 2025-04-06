using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class CreatedState : ISprintState
    {
        private readonly Sprint _sprint;

        public CreatedState(Sprint sprint)
        {
            _sprint = sprint;
        }

        public void AddBacklogItem(BacklogItem item)
        {
            _sprint.SprintBacklog.AddItem(item);
            // Koppel item aan observers van sprint? Nee, item is zelf subject.
            // Koppel sprint observers aan item? Ook niet nodig, item notificeert zelf.
            Console.WriteLine($"Added backlog item '{item.Title}' to sprint '{_sprint.Name}'.");
        }

        public void RemoveBacklogItem(BacklogItem item)
        {
            _sprint.SprintBacklog.RemoveItem(item);
            Console.WriteLine($"Removed backlog item '{item.Title}' from sprint '{_sprint.Name}'.");
            // Potentieel terug naar product backlog verplaatsen
        }

        public void ChangeName(string newName)
        {
            _sprint.Name = newName;
            _sprint.NotifyObservers($"Sprint name changed to '{newName}'.");
        }

        public void ChangeDates(DateTime newStart, DateTime newEnd)
        {
            if (newStart >= newEnd)
            {
                throw new ArgumentException("Start date must be before end date.");
            }
            _sprint.StartDate = newStart;
            _sprint.EndDate = newEnd;
            _sprint.NotifyObservers($"Sprint dates changed to {newStart:d} - {newEnd:d}.");
        }

        public void StartSprint()
        {
            // Kan alleen starten als er teamleden en backlog items zijn?
            if (!_sprint.TeamMembers.Any())
            {
                throw new InvalidStateException("Cannot start sprint: No team members assigned.");
            }
            if (!_sprint.SprintBacklog.Items.Any())
            {
                throw new InvalidStateException("Cannot start sprint: No backlog items in the sprint backlog.");
            }

            if (DateTime.Now < _sprint.StartDate)
            {
                throw new InvalidStateException($"Cannot start sprint '{_sprint.Name}' before its start date ({_sprint.StartDate:d}).");
            }

            _sprint.SetState(new RunningState(_sprint));
        }

        public void FinishSprint()
        {
            throw new InvalidStateException("Cannot finish sprint: Sprint has not started yet.");
        }

        public void StartRelease(Action<bool> releaseCallback)
        {
            throw new InvalidStateException("Cannot start release: Sprint has not started yet.");
        }

        public void CancelRelease()
        {
            throw new InvalidStateException("Cannot cancel release: No release in progress for a created sprint.");
        }


        public void CloseSprint()
        {
            throw new InvalidStateException("Cannot close sprint: Sprint has not finished or been released yet.");
        }

        public void ReviewSprint(string reviewDocumentPath)
        {
            throw new InvalidStateException("Cannot review sprint: Sprint has not finished yet.");
        }
    }
}