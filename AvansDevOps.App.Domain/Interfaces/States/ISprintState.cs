using AvansDevOps.App.Domain.Entities;
using System;

namespace AvansDevOps.App.Domain.Interfaces.States
{
    // State Pattern Interface
    public interface ISprintState
    {
        // Methods for managing backlog items (only allowed in certain states)
        void AddBacklogItem(BacklogItem item);
        void RemoveBacklogItem(BacklogItem item);

        // Methods for changing sprint properties (only allowed in certain states)
        void ChangeName(string newName);
        void ChangeDates(DateTime newStart, DateTime newEnd);

        // Methods for transitioning between states
        void StartSprint();
        void FinishSprint(); // Timer based or manual trigger
        void StartRelease(Action<bool> releaseCallback); // Start pipeline execution
        void CancelRelease();
        void CloseSprint(); // Final state after successful release
        void ReviewSprint(string reviewDocumentPath); // Specific action for review sprints

    }
}