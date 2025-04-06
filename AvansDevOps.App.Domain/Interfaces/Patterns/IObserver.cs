using System.Runtime.InteropServices.JavaScript;

namespace AvansDevOps.App.Domain.Interfaces.Patterns
{
    // Observer Pattern: Observer Interface
    public interface IObserver
    {
        // Ontvangt update van Subject
        // Subject wordt meegegeven voor context (wie stuurde de update?)
        void Update(ISubject subject, string message);
    }
}