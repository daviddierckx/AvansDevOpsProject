using AvansDevOps.App.Application.Services;
using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Factories;
using AvansDevOps.App.Domain.Interfaces.Repositories;
using AvansDevOps.App.Domain.Interfaces.Services;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using AvansDevOps.App.Infrastructure.Persistence.FakeRepositories;
using AvansDevOps.App.Infrastructure.Notifications;
using AvansDevOps.App.Infrastructure.Reporting;
using AvansDevOps.App.Infrastructure.Pipeline;
using AvansDevOps.App.Domain.Decorators; // Voor report decorators
using AvansDevOps.App.Domain.ValueObjects; // Voor ReportFormat
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvansDevOps.App.Domain.States.SprintStates;
using AvansDevOps.App.Domain.Interfaces.Patterns; // Voor async pipeline

namespace AvansDevOps.App
{
    class Program
    {
        static async Task Main(string[] args) // Main async maken voor pipeline
        {
            Console.WriteLine("Setting up Avans DevOps Simulation...");

            // --- Setup Infrastructure (Dependency Injection simulatie) ---
            // Repositories
            var projectRepo = new FakeProjectRepository();
            var sprintRepo = new FakeSprintRepository(); // Geen afhankelijkheden meer nodig
            var backlogItemRepo = new FakeBacklogItemRepository(sprintRepo); // Geef sprintRepo mee
            var userRepo = new FakeUserRepository();

            // Notification Strategies & Service
            var emailStrategy = new EmailNotificationStrategy();
            var slackStrategy = new SlackNotificationStrategy();
            var notificationStrategies = new List<INotificationStrategy> { emailStrategy, slackStrategy };
            var notificationService = new StubNotificationService(notificationStrategies);

            // Reporting Strategies
            var pdfStrategy = new PdfReportStrategy();
            var pngStrategy = new PngReportStrategy();
            var reportStrategies = new List<IReportGenerationStrategy> { pdfStrategy, pngStrategy };

            // Pipeline Executor
            var pipelineExecutor = new StubPipelineExecutor();

            // Application Services
            var sprintManager = new SprintManager(sprintRepo, notificationService, pipelineExecutor);
            var backlogItemManager = new BacklogItemManager(backlogItemRepo, userRepo, notificationService);


            // --- Setup Domain Objects ---
            Console.WriteLine("\n--- Creating Users ---");
            var productOwner = new ProductOwner("Alice", "alice@example.com", "alice_po");
            var scrumMaster = new ScrumMaster("Bob", "bob@example.com", "bob_sm");
            var dev1 = new Developer("Charlie", "charlie@example.com", "charlie_dev");
            var dev2 = new Developer("David", "david@example.com"); // Geen Slack
            var tester1 = new Developer("Eve (Tester)", "eve@example.com", "eve_tester"); // Gebruik Developer als Tester voor nu

            // Voeg notificatie voorkeuren toe (Strategy Pattern)
            productOwner.AddNotificationPreference(emailStrategy);
            scrumMaster.AddNotificationPreference(emailStrategy);
            scrumMaster.AddNotificationPreference(slackStrategy);
            dev1.AddNotificationPreference(slackStrategy);
            dev2.AddNotificationPreference(emailStrategy); // Alleen Email
            tester1.AddNotificationPreference(emailStrategy);

            userRepo.Add(productOwner);
            userRepo.Add(scrumMaster);
            userRepo.Add(dev1);
            userRepo.Add(dev2);
            userRepo.Add(tester1);

            Console.WriteLine("\n--- Creating Project ---");
            var project = new Project("Project Phoenix", productOwner);
            projectRepo.Add(project);

            Console.WriteLine("\n--- Creating Backlog Items ---");
            var item1 = backlogItemManager.CreateBacklogItem("User Login Feature", "Implement user authentication", 8);
            var item2 = backlogItemManager.CreateBacklogItem("Data Dashboard", "Display key metrics", 13);
            var item3 = backlogItemManager.CreateBacklogItem("Refactor Payment Module", "Improve code quality", 5);
            backlogItemManager.AddItemToProjectBacklog(project, item1);
            backlogItemManager.AddItemToProjectBacklog(project, item2);
            backlogItemManager.AddItemToProjectBacklog(project, item3);

            Console.WriteLine("\n--- Adding Activities to Item 1 ---");
            backlogItemManager.AddActivityToItem(item1.Id, "Create UI Mockup");
            backlogItemManager.AddActivityToItem(item1.Id, "Implement Backend Logic");
            backlogItemManager.AddActivityToItem(item1.Id, "Write Unit Tests");

            Console.WriteLine("\n--- Assigning Developers ---");
            backlogItemManager.AssignDeveloperToItem(item1.Id, dev1.Id);
            backlogItemManager.AssignDeveloperToItem(item2.Id, dev2.Id);
            // Item 3 nog niet toegewezen

            // Koppel observers (bv. Scrum Master aan items)
            item1.Attach(scrumMaster);
            item2.Attach(scrumMaster);
            item3.Attach(scrumMaster);
            // Tester observeren items die ReadyForTesting worden? Kan via Notificatie Service.

            Console.WriteLine("\n--- Creating Sprints ---");
            var sprint1 = sprintManager.CreateSprint("Sprint 1 - Foundation", DateTime.Now.AddDays(0), DateTime.Now.AddDays(0), SprintType.Review, scrumMaster, project);
            var sprint2 = sprintManager.CreateSprint("Sprint 2 - Release 1.0", DateTime.Now.AddDays(0), DateTime.Now.AddDays(0), SprintType.Release, scrumMaster, project);
            project.AddSprint(sprint1);
            project.AddSprint(sprint2);

            Console.WriteLine("\n--- Adding Team Members and Items to Sprint 1 ---");
            // Moet gebeuren voordat Sprint gestart wordt (in CreatedState)
            try
            {
                sprint1.AddTeamMember(dev1); // OK
                sprint1.AddTeamMember(dev2); // OK
                sprintManager.AddBacklogItemToSprint(sprint1.Id, item1); // OK
                sprintManager.AddBacklogItemToSprint(sprint1.Id, item3); // OK (onassigned item)
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }


            Console.WriteLine("\n--- Starting Sprint 1 ---");
            try
            {
                sprintManager.StartSprint(sprint1.Id); // Verandert state naar Running
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }

            // Probeer teamlid toe te voegen aan lopende sprint (moet falen)
            Console.WriteLine("\n--- Attempting to add member to running sprint (should fail) ---");
            try
            {
                sprint1.AddTeamMember(tester1);
            }
            catch (Exception ex) { Console.WriteLine($"EXPECTED ERROR: {ex.Message}"); }


            Console.WriteLine("\n--- Working on Backlog Item 1 ---");
            // Flow voor item 1 (Composite & State Pattern)
            item1.Display(); // Toon item en activities (Composite)
            try
            {
                backlogItemManager.StartTask(item1.Id); // Naar Doing
                                                        // Markeer activities als done (simulatie)
                var activity1 = item1.Activities[0]; activity1.MarkAsDone(); backlogItemManager.MarkActivityAsDone(item1.Id, activity1.Id); // Fake IDs
                var activity2 = item1.Activities[1]; activity2.MarkAsDone(); backlogItemManager.MarkActivityAsDone(item1.Id, activity2.Id);
                var activity3 = item1.Activities[2]; activity3.MarkAsDone(); backlogItemManager.MarkActivityAsDone(item1.Id, activity3.Id);

                backlogItemManager.MarkAsReadyForTesting(item1.Id); // Naar ReadyForTesting (notifies testers via observer/service)
                                                                    // Simulatie: Tester (Eve) pakt het op
                Console.WriteLine($"\n{tester1.Name} starts testing item '{item1.Title}'...");
                backlogItemManager.StartTesting(item1.Id); // Naar Testing
                                                           // Simulatie: Test slaagt
                Console.WriteLine($"\n{tester1.Name} finishes testing item '{item1.Title}' (Result: Passed).");
                backlogItemManager.SendTestingResult(item1.Id, true); // Naar Tested
                backlogItemManager.CompleteTask(item1.Id); // Naar Done
            }
            catch (Exception ex) { Console.WriteLine($"ERROR processing item 1: {ex.Message}"); }
            item1.Display();

            Console.WriteLine("\n--- Adding Discussion to Item 1 (should fail if Done) ---");
            try
            {
                backlogItemManager.AddDiscussionMessage(item1.Id, dev1, "Just confirming the implementation details.");
            }
            catch (Exception ex) { Console.WriteLine($"EXPECTED ERROR: {ex.Message}"); }

            Console.WriteLine("\n--- Working on Backlog Item 3 (Rejection Flow) ---");
            try
            {
                backlogItemManager.AssignDeveloperToItem(item3.Id, dev2.Id);
                backlogItemManager.StartTask(item3.Id); // Naar Doing
                backlogItemManager.MarkAsReadyForTesting(item3.Id); // Naar ReadyForTesting
                Console.WriteLine($"\n{tester1.Name} starts testing item '{item3.Title}'...");
                backlogItemManager.StartTesting(item3.Id); // Naar Testing
                                                           // Simulatie: Test faalt
                Console.WriteLine($"\n{tester1.Name} finishes testing item '{item3.Title}' (Result: Failed).");
                backlogItemManager.SendTestingResult(item3.Id, false); // Naar Todo (Notifies Dev & SM)
            }
            catch (Exception ex) { Console.WriteLine($"ERROR processing item 3: {ex.Message}"); }
            item3.Display();

            Console.WriteLine("\n--- Developer fixes and re-submits Item 3 ---");
            try
            {
                backlogItemManager.StartTask(item3.Id); // Naar Doing
                backlogItemManager.MarkAsReadyForTesting(item3.Id); // Naar ReadyForTesting
                Console.WriteLine($"\n{tester1.Name} starts testing item '{item3.Title}' again...");
                backlogItemManager.StartTesting(item3.Id); // Naar Testing
                                                           // Simulatie: Test slaagt nu
                Console.WriteLine($"\n{tester1.Name} finishes testing item '{item3.Title}' (Result: Passed).");
                backlogItemManager.SendTestingResult(item3.Id, true); // Naar Tested
                backlogItemManager.CompleteTask(item3.Id); // Naar Done
            }
            catch (Exception ex) { Console.WriteLine($"ERROR processing item 3 (retry): {ex.Message}"); }
            item3.Display();


            Console.WriteLine("\n--- Finishing Sprint 1 ---");
            try
            {
                sprintManager.FinishSprint(sprint1.Id); // Naar Finished
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }

            Console.WriteLine("\n--- Reviewing Sprint 1 (Review Sprint) ---");
            try
            {
                sprintManager.ReviewSprint(sprint1.Id, "/path/to/sprint1_review_summary.docx"); // Naar Reviewed
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }

            Console.WriteLine("\n--- Closing Sprint 1 ---");
            try
            {
                sprintManager.CloseSprint(sprint1.Id); // Afronden
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }


            Console.WriteLine("\n--- Preparing Sprint 2 (Release Sprint) ---");
            // Maak een pipeline (Factory Pattern)
            var pipeline = new DevelopmentPipeline("Release Pipeline v1.0");
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Source, "Get Source", new Dictionary<string, object> { { "RepositoryUrl", "git://github.com/myorg/phoenix.git" }, { "Branch", "main" } }));
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Package, "Install Deps", new Dictionary<string, object> { { "Packages", new List<string> { "Newtonsoft.Json", "Serilog" } } }));
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Build, "Build Solution", new Dictionary<string, object> { { "Configuration", "Release" } }));
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Test, "Run Unit Tests", new Dictionary<string, object> { { "CollectCoverage", true } })); // Success test
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Analyse, "Static Analysis", new Dictionary<string, object> { { "Tool", "SonarScanner" } }));
            pipeline.AddAction(PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Deploy, "Deploy to Prod", new Dictionary<string, object> { { "Environment", "Production" }, { "ServerAddress", "prod.server.com" } })); // Success deploy

            sprint2.Pipeline = pipeline; // Koppel pipeline aan sprint
            sprintRepo.Update(sprint2); // Sla wijziging op

            try
            {
                sprint2.AddTeamMember(dev1);
                sprint2.AddTeamMember(dev2); // Andere teamleden?
                sprintManager.AddBacklogItemToSprint(sprint2.Id, item2); // Item 2 in deze sprint
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }

            Console.WriteLine("\n--- Starting Sprint 2 ---");
            try
            {
                sprintManager.StartSprint(sprint2.Id);
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }


            Console.WriteLine("\n--- Working on Backlog Item 2 ---");
            try
            {
                backlogItemManager.StartTask(item2.Id);
                backlogItemManager.MarkAsReadyForTesting(item2.Id);
                Console.WriteLine($"\n{tester1.Name} starts testing item '{item2.Title}'...");
                backlogItemManager.StartTesting(item2.Id);
                Console.WriteLine($"\n{tester1.Name} finishes testing item '{item2.Title}' (Result: Passed).");
                backlogItemManager.SendTestingResult(item2.Id, true);
                backlogItemManager.CompleteTask(item2.Id);
            }
            catch (Exception ex) { Console.WriteLine($"ERROR processing item 2: {ex.Message}"); }
            item2.Display();

            Console.WriteLine("\n--- Finishing Sprint 2 ---");
            try
            {
                sprintManager.FinishSprint(sprint2.Id);
            }
            catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }

            Console.WriteLine("\n--- Initiating Release for Sprint 2 ---");
            // Dit start de pipeline simulatie asynchroon
            sprintManager.InitiateRelease(sprint2.Id);

            Console.WriteLine("\n--- Main thread continues while pipeline runs (simulation)... ---");
            // Wacht expliciet tot de pipeline klaar is in deze demo
            Console.WriteLine("--- Waiting for pipeline simulation to complete... (Max 5 seconds) ---");
            await Task.Delay(TimeSpan.FromSeconds(5)); // Geef pipeline tijd om te voltooien


            Console.WriteLine($"\n--- Current state of Sprint 2 after pipeline: {sprint2.CurrentState.GetType().Name} ---");

            // Als het goed is gegaan, staat de sprint nu in ReleasedState
            if (sprint2.CurrentState is ReleasedState)
            {
                Console.WriteLine("\n--- Closing Sprint 2 (after successful release) ---");
                try
                {
                    sprintManager.CloseSprint(sprint2.Id);
                }
                catch (Exception ex) { Console.WriteLine($"ERROR: {ex.Message}"); }
            }
            else
            {
                Console.WriteLine("\n--- Sprint 2 did not end in Released state. Manual intervention needed? ---");
                // Optie: Cancel de release als die faalde en terugging naar Finished
                if (sprint2.CurrentState is FinishedState)
                {
                    Console.WriteLine("--- Release pipeline likely failed. Cancelling release (manual step simulation)... ---");
                    // In een echt scenario zou PO/SM dit doen.
                    // We simuleren het niet hier om de flow duidelijk te houden.
                    // sprintManager.CancelRelease(sprint2.Id); // Zou naar Cancelled gaan
                    // sprintManager.CloseSprint(sprint2.Id);
                }
            }


            Console.WriteLine("\n--- Generating Reports (Decorator & Strategy Pattern) ---");
            // Maak een basis rapport component
            var baseReport = new Report("Sprint 1 Performance Report", sprint1); // Gebruik sprint 1 die al klaar is

            // Decoreer het rapport (Decorator Pattern)
            IReportComponent decoratedReport = new HeaderDecorator(
                new FooterDecorator(baseReport, "Avans DevOps Internal Use Only"),
                "Avans University - ICT Academy"
            );

            // Genereer in verschillende formaten (Strategy Pattern)
            var reportGenerator = new ReportGenerator(reportStrategies); // Geef beschikbare strategieën mee
            reportGenerator.Generate(decoratedReport, ReportFormat.Pdf, "sprint1_report"); // Genereert sprint1_report.pdf
            reportGenerator.Generate(decoratedReport, ReportFormat.Png, "sprint1_summary"); // Genereert sprint1_summary.png


            Console.WriteLine("\n--- Simulation Finished ---");
        }
    }

    // Helper class om report generation strategie te kiezen
    public class ReportGenerator
    {
        private readonly Dictionary<ReportFormat, IReportGenerationStrategy> _strategies;

        public ReportGenerator(IEnumerable<IReportGenerationStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.GetFormat());
        }

        public void Generate(IReportComponent reportComponent, ReportFormat format, string baseFilePath)
        {
            if (_strategies.TryGetValue(format, out var strategy))
            {
                strategy.GenerateReport(reportComponent, baseFilePath);
            }
            else
            {
                Console.WriteLine($"ERROR: No report generation strategy found for format {format}.");
            }
        }
    }
}