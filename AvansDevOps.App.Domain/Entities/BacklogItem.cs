using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.States.BacklogItemStates;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System;

namespace AvansDevOps.App.Domain.Entities
{
    // BacklogItem is een ISubject voor het Observer Pattern (bijv. voor notificaties)
    // BacklogItem is een IWorkItem voor het Composite Pattern (samen met Activity)
    public class BacklogItem : ISubject, IWorkItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Developer AssignedDeveloper { get; set; } // Max 1 developer per item
        public List<Activity> Activities { get; private set; }
        public List<DiscussionThread> DiscussionThreads { get; private set; }
        public IBacklogItemState CurrentState { get; private set; } // State Pattern
        public int StoryPoints { get; set; } // Optioneel, voor effort tracking

        private List<IObserver> _observers = new List<IObserver>(); // Observer Pattern

        public BacklogItem(string title, string description, int storyPoints = 0)
        {
            Title = title;
            Description = description;
            StoryPoints = storyPoints;
            Activities = new List<Activity>();
            DiscussionThreads = new List<DiscussionThread>();
            // Initial state: ToDo (State Pattern)
            CurrentState = new TodoState(this);
        }

        public void SetState(IBacklogItemState newState) // State Pattern
        {
            CurrentState = newState;
            Console.WriteLine($"Backlog Item '{Title}' changed state to: {newState.GetType().Name}");
            // Notify observers about state change (Observer Pattern)
            NotifyObservers($"Backlog Item '{Title}' state changed to {newState.GetType().Name}");
        }

        // State Pattern Methods - delegeren naar het state object
        public void StartTask() => CurrentState.StartTask();
        public void MarkAsReadyForTesting() => CurrentState.MarkAsReadyForTesting();
        public void StartTesting() => CurrentState.StartTesting();
        public void SendTestingResult(bool passed) => CurrentState.SendTestingResult(passed);
        public void CompleteTask() => CurrentState.CompleteTask();
        public void ReopenTask() => CurrentState.ReopenTask();

        public void AddActivity(Activity activity) // Composite Pattern
        {
            Activities.Add(activity);
        }

        public void RemoveActivity(Activity activity) // Composite Pattern
        {
            Activities.Remove(activity);
        }

        public bool IsDone() // Composite Pattern & State Pattern
        {
            // Een BacklogItem is done als zijn state DoneState is EN alle activiteiten ook done zijn.
            bool allActivitiesDone = Activities.All(a => a.IsDone());
            return CurrentState is DoneState && allActivitiesDone;
        }

        public void AddDiscussionThread(DiscussionThread thread)
        {
            if (CurrentState is DoneState) // Regel uit casus
            {
                throw new InvalidOperationException("Cannot add discussion threads to a completed backlog item.");
            }
            DiscussionThreads.Add(thread);
        }

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
            // Notify assigned developer if exists
            if (AssignedDeveloper != null)
            {
                AssignedDeveloper.Update(this, message); // Observer Pattern Call
            }

            // Notify other relevant observers (e.g., Scrum Master, Testers for certain states)
            foreach (var observer in _observers.ToList()) // ToList to avoid modification issues during iteration
            {
                observer.Update(this, message); // Observer Pattern Call
            }
        }
        // --- End Observer Pattern ---

        // --- Composite Pattern Implementation ---
        public void Display(int indentLevel = 0)
        {
            Console.WriteLine($"{new string(' ', indentLevel * 2)}- Backlog Item: {Title} [{CurrentState.GetType().Name}] {(AssignedDeveloper != null ? $"(Assigned: {AssignedDeveloper.Name})" : "")}");
            foreach (var activity in Activities)
            {
                activity.Display(indentLevel + 1);
            }
        }
        // --- End Composite Pattern ---
    }
}