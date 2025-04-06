namespace AvansDevOps.App.Domain.Interfaces.States
{
    // State Pattern Interface
    public interface IBacklogItemState
    {
        void StartTask();
        void MarkAsReadyForTesting();
        void StartTesting();
        void SendTestingResult(bool passed);
        void CompleteTask();
        void ReopenTask(); // Terug naar Todo
    }
}