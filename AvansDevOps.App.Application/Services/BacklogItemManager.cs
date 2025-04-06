using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using AvansDevOps.App.Domain.Interfaces.Services;
using AvansDevOps.App.Domain.States.BacklogItemStates; // Nodig voor DoneState check
using System;
using System.Collections.Generic;

namespace AvansDevOps.App.Application.Services
{
    // Application Service - use cases gerelateerd aan Backlog Items
    public class BacklogItemManager
    {
        private readonly IBacklogItemRepository _backlogItemRepository;
        private readonly IUserRepository _userRepository; // Nodig om developer toe te wijzen
        private readonly INotificationService _notificationService;

        public BacklogItemManager(IBacklogItemRepository backlogItemRepository, IUserRepository userRepository, INotificationService notificationService)
        {
            _backlogItemRepository = backlogItemRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public BacklogItem CreateBacklogItem(string title, string description, int storyPoints = 0)
        {
            var item = new BacklogItem(title, description, storyPoints);
            _backlogItemRepository.Add(item);
            return item;
        }

        public void AddItemToProjectBacklog(Project project, BacklogItem item)
        {
            // Aanname: item is al gecreëerd en opgeslagen via CreateBacklogItem
            project.AddBacklogItem(item);
            // Geen repo update nodig hier, project beheert zijn eigen backlog (of IProjectRepository update)
        }

        public void AssignDeveloperToItem(int itemId, int developerId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");

            var user = _userRepository.GetById(developerId);
            if (user == null || !(user is Developer developer))
            {
                throw new KeyNotFoundException($"Developer with ID {developerId} not found.");
            }

            item.AssignedDeveloper = developer;
            _backlogItemRepository.Update(item);

            // Notificeer developer
            _notificationService.SendNotification($"You have been assigned to backlog item: '{item.Title}'", developer);
        }

        public void AddActivityToItem(int itemId, string activityDescription)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");

            var activity = new Activity(activityDescription);
            item.AddActivity(activity); // Dit voegt alleen toe aan de list in memory
            _backlogItemRepository.Update(item); // Zorg dat de wijziging (impliciet via item state) opgeslagen wordt
                                                 // In een echte DB zou je Activity apart opslaan met FK naar item.
        }

        public void MarkActivityAsDone(int itemId, int activityId) // Vereist mogelijk IActivityRepository
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");

            var activity = item.Activities.FirstOrDefault(a => a.Id == activityId); // Simpele zoektocht
            if (activity == null) throw new KeyNotFoundException($"Activity with ID {activityId} not found in item {itemId}.");

            activity.MarkAsDone();
            _backlogItemRepository.Update(item); // Sla wijziging op
                                                 // Notificatie?
            if (item.AssignedDeveloper != null)
                _notificationService.SendNotification($"Activity '{activity.Description}' for item '{item.Title}' marked as done.", item.AssignedDeveloper);

        }

        // --- State Transition Methods ---
        public void StartTask(int itemId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");
            item.StartTask();
            _backlogItemRepository.Update(item);
            // Notificaties worden afgehandeld door de SetState/NotifyObservers in BacklogItem
        }

        public void MarkAsReadyForTesting(int itemId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");
            // Extra check: alle activities moeten klaar zijn? Casus specificeert dit niet expliciet voor deze stap.
            // Laten we het voor nu toestaan.
            // if (!item.Activities.All(a => a.IsDone()))
            // {
            //     throw new InvalidOperationException("Cannot mark as ready for testing: Not all activities are completed.");
            // }
            item.MarkAsReadyForTesting();
            _backlogItemRepository.Update(item);
            // Notificatie naar Testers (via Observer pattern in BacklogItem)
            // We moeten hier mogelijk expliciet testers notificeren via de service
            // als ze niet direct aan het item hangen. Vereist UserRepository.GetAllTesters()
        }

        public void StartTesting(int itemId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");
            item.StartTesting();
            _backlogItemRepository.Update(item);
        }

        public void SendTestingResult(int itemId, bool passed)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");
            item.SendTestingResult(passed);
            _backlogItemRepository.Update(item);
            // Notificaties afgehandeld in BacklogItem (naar dev/SM bij falen)
        }

        public void CompleteTask(int itemId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");

            // Extra check: Moeten alle activiteiten Done zijn? Ja, zie IsDone() logica.
            if (!item.Activities.All(a => a.IsDone()))
            {
                throw new InvalidOperationException($"Cannot complete item '{item.Title}': Not all its activities are marked as done.");
            }

            item.CompleteTask(); // Gaat naar DoneState
            _backlogItemRepository.Update(item);
        }

        public void ReopenTask(int itemId)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");
            item.ReopenTask(); // Gaat terug naar TodoState
            _backlogItemRepository.Update(item);
        }
        // --- End State Transition Methods ---

        public void AddDiscussionMessage(int itemId, User author, string messageContent)
        {
            var item = _backlogItemRepository.GetById(itemId);
            if (item == null) throw new KeyNotFoundException($"Backlog item with ID {itemId} not found.");

            // Zoek bestaande thread of maak nieuwe (simpele aanpak: 1 thread per item)
            var thread = item.DiscussionThreads.FirstOrDefault();
            if (thread == null)
            {
                thread = new DiscussionThread($"Discussion for '{item.Title}'", item);
                item.AddDiscussionThread(thread); // Check in AddDiscussionThread of item niet Done is
            }

            var message = new Message(messageContent, author);
            thread.AddMessage(message); // Check in AddMessage of item niet Done is

            _backlogItemRepository.Update(item); // Sla wijzigingen op

            // Notificaties afgehandeld binnen DiscussionThread.AddMessage
        }


        public BacklogItem GetBacklogItem(int id)
        {
            return _backlogItemRepository.GetById(id);
        }

        public IEnumerable<BacklogItem> GetAllBacklogItems()
        {
            return _backlogItemRepository.GetAll();
        }

        public IEnumerable<BacklogItem> GetSprintBacklogItems(int sprintId)
        {
            // Dit kan via SprintRepository of BacklogItemRepository
            return _backlogItemRepository.GetItemsBySprint(sprintId);
        }
    }
}