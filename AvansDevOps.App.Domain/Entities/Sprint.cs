using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.States.SprintStates;
using AvansDevOps.App.Domain.Exceptions;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

namespace AvansDevOps.App.Domain.Entities
{
    public enum SprintType { Review, Release }

    // Sprint is een ISubject voor het Observer Pattern (bijv. voor notificaties)
    public class Sprint : ISubject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintType Type { get; private set; }
        public SprintBacklog SprintBacklog { get; private set; }
        public ScrumMaster ScrumMaster { get; set; }
        public List<Developer> TeamMembers { get; private set; }
        public ISprintState CurrentState { get; private set; } // State Pattern
        public DevelopmentPipeline Pipeline { get; set; } // Optioneel, gekoppeld voor Release sprints
        public string ReviewSummaryDocumentPath { get; set; } // Voor Review sprints

        private List<IObserver> _observers = new List<IObserver>(); // Observer Pattern
        private Project _project; // Referentie naar het project voor context (bv. Product Owner)

        public Sprint(string name, DateTime startDate, DateTime endDate, SprintType type, ScrumMaster scrumMaster, Project project)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Type = type;
            ScrumMaster = scrumMaster; // Moet aanwezig zijn
            _project = project; // Project referentie opslaan

            SprintBacklog = new SprintBacklog();
            TeamMembers = new List<Developer>();
            // Initiële staat (State Pattern)
            CurrentState = new CreatedState(this);

            // Scrum Master en Product Owner observeren de Sprint (Observer Pattern)
            Attach(scrumMaster);
            if (project.ProductOwner != null)
            {
                Attach(project.ProductOwner);
            }
        }

        public ProductOwner GetProductOwner() // Helper om PO te krijgen
        {
            return _project?.ProductOwner;
        }

        public void AddTeamMember(Developer developer)
        {
            if (CurrentState is CreatedState) // Alleen toevoegen in Created state
            {
                if (!TeamMembers.Contains(developer))
                {
                    TeamMembers.Add(developer);
                    Attach(developer); // Teamleden observeren ook de sprint
                }
            }
            else
            {
                throw new InvalidStateException("Cannot add team members when the sprint is not in the 'Created' state.");
            }
        }

        public void RemoveTeamMember(Developer developer)
        {
            if (CurrentState is CreatedState)
            {
                if (TeamMembers.Remove(developer))
                {
                    Detach(developer); // Verwijder als observer
                }
            }
            else
            {
                throw new InvalidStateException("Cannot remove team members when the sprint is not in the 'Created' state.");
            }
        }

        public void AddBacklogItem(BacklogItem item)
        {
            CurrentState.AddBacklogItem(item); // Delegatie naar State
        }

        public void RemoveBacklogItem(BacklogItem item)
        {
            CurrentState.RemoveBacklogItem(item); // Delegatie naar State
        }

        public void ChangeName(string newName)
        {
            CurrentState.ChangeName(newName); // Delegatie naar State
        }
        public void ChangeDates(DateTime newStart, DateTime newEnd)
        {
            CurrentState.ChangeDates(newStart, newEnd); // Delegatie naar State
        }


        public void SetState(ISprintState newState) // State Pattern
        {
            CurrentState = newState;
            Console.WriteLine($"Sprint '{Name}' changed state to: {newState.GetType().Name}");
            NotifyObservers($"Sprint '{Name}' state changed to {newState.GetType().Name}"); // Observer Pattern
        }

        // State Pattern Methods - delegeren naar het state object
        public void StartSprint() => CurrentState.StartSprint();
        public void FinishSprint() => CurrentState.FinishSprint();
        public void StartRelease(Action<bool> releaseCallback) => CurrentState.StartRelease(releaseCallback);
        public void CancelRelease() => CurrentState.CancelRelease();
        public void CloseSprint() => CurrentState.CloseSprint(); // Voor na Release
        public void ReviewSprint(string reviewDocumentPath) => CurrentState.ReviewSprint(reviewDocumentPath); // Voor Review

        // --- Observer Pattern Implementation ---
        public void Attach(IObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(string message)
        {
            Console.WriteLine($"--- Sprint Notification for '{Name}': {message} ---");
            // Kopieer lijst om modificatie tijdens iteratie te voorkomen
            var currentObservers = _observers.ToList();
            foreach (var observer in currentObservers)
            {
                // Stuur bericht met sprint als context
                observer.Update(this, message); // Observer Pattern Call
            }
            Console.WriteLine($"--- End Sprint Notification ---");
        }
        // --- End Observer Pattern ---
    }
}