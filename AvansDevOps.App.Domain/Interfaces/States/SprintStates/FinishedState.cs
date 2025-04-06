using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System;

namespace AvansDevOps.App.Domain.States.SprintStates
{
    // State Pattern: Concrete State
    public class FinishedState : ISprintState
    {
        private readonly Sprint _sprint;

        public FinishedState(Sprint sprint)
        {
            _sprint = sprint;
        }

        // Cannot change details after finishing
        public void AddBacklogItem(BacklogItem item) => throw new InvalidStateException("Cannot modify sprint backlog after sprint is finished.");
        public void RemoveBacklogItem(BacklogItem item) => throw new InvalidStateException("Cannot modify sprint backlog after sprint is finished.");
        public void ChangeName(string newName) => throw new InvalidStateException("Cannot change sprint details after sprint is finished.");
        public void ChangeDates(DateTime newStart, DateTime newEnd) => throw new InvalidStateException("Cannot change sprint details after sprint is finished.");
        public void StartSprint() => throw new InvalidStateException("Cannot start sprint: Sprint is already finished.");
        public void FinishSprint() => throw new InvalidStateException("Cannot finish sprint: Sprint is already finished.");


        public void StartRelease(Action<bool> releaseCallback)
        {
            // Kan alleen als het een Release sprint is en er een pipeline is
            if (_sprint.Type != SprintType.Release)
            {
                throw new InvalidStateException($"Cannot start release: Sprint '{_sprint.Name}' is a Review sprint, not a Release sprint.");
            }
            if (_sprint.Pipeline == null)
            {
                throw new InvalidStateException($"Cannot start release: No pipeline associated with sprint '{_sprint.Name}'.");
            }
            // Optioneel: check of alle items Done zijn? Casus zegt: PO kan release starten.
            // if (!_sprint.SprintBacklog.AllItemsDone())
            // {
            //     throw new InvalidStateException($"Cannot start release: Not all backlog items in sprint '{_sprint.Name}' are Done.");
            // }

            _sprint.SetState(new ReleasingState(_sprint, releaseCallback));
            // Start pipeline execution (handled within ReleasingState constructor or separate trigger)
            // releaseCallback will be invoked by the pipeline executor later
        }

        public void CancelRelease()
        {
            throw new InvalidStateException("Cannot cancel release: No release in progress for a finished sprint.");
        }

        public void CloseSprint()
        {
            // Dit is een optie als de release NIET gedaan wordt of als het een Review sprint is
            // die direct gesloten wordt na review. Echter, de flow is meestal via Released or Reviewed.
            // Laten we CloseSprint alleen toestaan vanuit Released/Reviewed/Cancelled state.
            throw new InvalidStateException("Cannot close sprint directly from Finished state. Sprint needs to be Released or Reviewed first (or Cancelled).");
        }

        public void ReviewSprint(string reviewDocumentPath)
        {
            if (_sprint.Type != SprintType.Review)
            {
                throw new InvalidStateException($"Cannot review sprint: Sprint '{_sprint.Name}' is a Release sprint, not a Review sprint.");
            }
            if (string.IsNullOrWhiteSpace(reviewDocumentPath))
            {
                throw new ArgumentException("Review document path cannot be empty.");
            }

            _sprint.ReviewSummaryDocumentPath = reviewDocumentPath;
            _sprint.SetState(new ReviewedState(_sprint));
            // Notificatie naar PO?
            _sprint.NotifyObservers($"Sprint '{_sprint.Name}' has been reviewed. Summary: {reviewDocumentPath}");
        }
    }
}