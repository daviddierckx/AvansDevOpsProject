using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using AvansDevOps.App.Domain.Interfaces.Services; // Voor INotificationService
using AvansDevOps.App.Infrastructure.Pipeline; // Voor StubPipelineExecutor
using System;
using System.Collections.Generic;

namespace AvansDevOps.App.Application.Services
{
    // Application Service -  use cases gerelateerd aan Sprints
    public class SprintManager
    {
        private readonly ISprintRepository _sprintRepository;
        private readonly INotificationService _notificationService; // Injecteer notificatie service
        private readonly StubPipelineExecutor _pipelineExecutor; // Injecteer pipeline executor

        public SprintManager(ISprintRepository sprintRepository, INotificationService notificationService, StubPipelineExecutor pipelineExecutor)
        {
            _sprintRepository = sprintRepository;
            _notificationService = notificationService;
            _pipelineExecutor = pipelineExecutor;
        }

        public Sprint CreateSprint(string name, DateTime start, DateTime end, SprintType type, ScrumMaster sm, Project project)
        {
            // Validatie
            if (end <= start) throw new ArgumentException("End date must be after start date.");

            var sprint = new Sprint(name, start, end, type, sm, project);
            _sprintRepository.Add(sprint); // Opslaan

            // Notificatie (optioneel)
            _notificationService.SendNotification($"New sprint '{name}' created.", sm);
            if (project.ProductOwner != null)
                _notificationService.SendNotification($"New sprint '{name}' created.", project.ProductOwner);


            return sprint;
        }

        public void AddBacklogItemToSprint(int sprintId, BacklogItem item)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.AddBacklogItem(item);
                _sprintRepository.Update(sprint); // Update state in repo
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding item to sprint: {ex.Message}");
                // Afhankelijk van de exception, eventueel terugdraaien of loggen
                throw; 
            }
        }

        public void StartSprint(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.StartSprint();
                _sprintRepository.Update(sprint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting sprint: {ex.Message}");
                throw;
            }
        }

        public void FinishSprint(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.FinishSprint();
                _sprintRepository.Update(sprint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finishing sprint: {ex.Message}");
                throw;
            }
        }

        public void InitiateRelease(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                // De callback die wordt aangeroepen als de pipeline klaar is
                Action<bool> releaseCallback = (success) =>
                {
                    // Dit wordt uitgevoerd *nadat* de pipeline klaar is.
                    // De state change gebeurt in HandleReleaseResult van de ReleasingState.
                    Console.WriteLine($"--- Release Callback for Sprint {sprintId} ---");
                    Console.WriteLine($"Pipeline execution finished. Success: {success}");
                    // Update de repo nadat de state is veranderd door HandleReleaseResult
                    _sprintRepository.Update(sprint);
                    Console.WriteLine($"Sprint {sprintId} state after callback: {sprint.CurrentState.GetType().Name}");
                    Console.WriteLine($"--- End Release Callback ---");

                    // Verstuur notificaties via de service na de state change
                    string resultMessage = success ? "successfully released" : "release failed";
                    var recipients = new List<User> { sprint.ScrumMaster };
                    if (sprint.GetProductOwner() != null) recipients.Add(sprint.GetProductOwner());
                    _notificationService.SendNotificationToMultiple($"Sprint '{sprint.Name}' {resultMessage}.", recipients);

                };

                // Start de state transition naar Releasing.
                // De ReleasingState constructor/trigger zou de pipeline moeten starten.
                sprint.StartRelease(releaseCallback); // Dit verandert de state naar Releasing
                _sprintRepository.Update(sprint); // Sla de Releasing state op

                // Trigger de pipeline execution (gesimuleerd)
                // In een echt systeem zou dit asynchroon zijn.
                Console.WriteLine($"*** SprintManager: Triggering pipeline execution for Sprint {sprintId}... ***");
                _pipelineExecutor.ExecutePipelineAsync(sprint.Pipeline, sprint); // Start simulatie
                Console.WriteLine($"*** SprintManager: Pipeline execution started (async simulation). Waiting for callback... ***");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initiating release for sprint {sprintId}: {ex.Message}");
                // Zorg dat de sprint state correct is als StartRelease faalt (bv. terug naar Finished?)
                // Momenteel gooit StartRelease een exception als het niet kan.
                throw;
            }
        }

        public void CancelRelease(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.CancelRelease(); // Dit verandert state naar Cancelled en roept callback(false) aan
                _sprintRepository.Update(sprint);
                // Notificatie afgehandeld in CancelRelease/callback
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling release for sprint {sprintId}: {ex.Message}");
                throw;
            }
        }

        public void ReviewSprint(int sprintId, string reviewDocumentPath)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.ReviewSprint(reviewDocumentPath);
                _sprintRepository.Update(sprint);
                // Notificatie afgehandeld in ReviewSprint
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reviewing sprint {sprintId}: {ex.Message}");
                throw;
            }
        }

        public void CloseSprint(int sprintId)
        {
            var sprint = _sprintRepository.GetById(sprintId);
            if (sprint == null) throw new KeyNotFoundException($"Sprint with ID {sprintId} not found.");

            try
            {
                sprint.CloseSprint();
                _sprintRepository.Update(sprint); // Of verwijderen/archiveren? Update voor nu.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing sprint {sprintId}: {ex.Message}");
                throw;
            }
        }

        public Sprint GetSprint(int sprintId)
        {
            return _sprintRepository.GetById(sprintId);
        }

        public IEnumerable<Sprint> GetAllSprints()
        {
            return _sprintRepository.GetAll();
        }
    }
}