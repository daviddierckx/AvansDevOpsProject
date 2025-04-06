namespace AvansDevOps.App.Domain.Interfaces.Patterns
{
    // Observer Pattern: Subject Interface
    public interface ISubject
    {
        void Attach(IObserver observer); // Registreer observer
        void Detach(IObserver observer); // Verwijder observer
        void NotifyObservers(string message); // Stel observers op de hoogte
    }
}