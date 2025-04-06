
using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.States.SprintStates;    // Voor state check
using AvansDevOps.App.Domain.Exceptions;
using AvansDevOps.App.Domain.Interfaces.Patterns; // Voor IObserver/ISubject
using Moq; // Voor mocking
using System;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class SprintTests
    {
        // --- Hulpfuncties ---
        private ScrumMaster CreateScrumMaster() => new ScrumMaster("Test SM", "sm@test.com");
        private ProductOwner CreateProductOwner() => new ProductOwner("Test PO", "po@test.com");
        private Project CreateProject() => new Project("Test Project", CreateProductOwner());
        private Developer CreateDeveloper(string name = "Test Dev") => new Developer(name, name + "@test.com");
        private BacklogItem CreateBacklogItem(string title = "Test Item") => new BacklogItem(title, "Desc");

        private Sprint CreateNewReviewSprint()
        {
            var sm = CreateScrumMaster();
            var project = CreateProject();
            // Koppel PO en SM als observers
            var sprint = new Sprint("Review Sprint", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), SprintType.Review, sm, project);
            // Sprint constructor koppelt SM en PO al als observer
            return sprint;
        }

        private Sprint CreateNewReleaseSprint(bool withPipeline = true)
        {
            var sm = CreateScrumMaster();
            var project = CreateProject();
            var sprint = new Sprint("Release Sprint", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), SprintType.Release, sm, project);
            if (withPipeline)
            {
                // Voeg een dummy pipeline toe voor release tests
                sprint.Pipeline = new DevelopmentPipeline("Dummy Pipeline");
                // Voeg minimaal één actie toe om pipeline niet leeg te laten zijn
                sprint.Pipeline.AddAction(new BuildAction("Dummy Build"));
            }
            return sprint;
        }

        // --- Tests ---

        [Fact]
        // Requirement: FR04 (impliciet: sprint start in Created)
        public void Nieuwe_Sprint_Heeft_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();

            // Assert
            Assert.IsType<CreatedState>(sprint.CurrentState);
        }

        [Fact]
        // Requirement: FR04 (regel: teamleden toevoegen voor start)
        public void AddTeamMember_Voegt_Toe_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var dev = CreateDeveloper();

            // Act
            sprint.AddTeamMember(dev);

            // Assert
            Assert.Contains(dev, sprint.TeamMembers);
        }

        [Fact]
        // Requirement: FR04 (regel: teamleden niet toevoegen na start)
        public void AddTeamMember_Gooit_Exception_In_RunningState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var dev1 = CreateDeveloper("Dev1");
            var item1 = CreateBacklogItem("Item1");
            sprint.AddTeamMember(dev1); // Toevoegen in Created state
            sprint.AddBacklogItem(item1); // Item toevoegen
            sprint.StartSprint(); // Naar RunningState
            var dev2 = CreateDeveloper("Dev2");

            // Act & Assert
            Assert.IsType<RunningState>(sprint.CurrentState); // Verifieer state
            var exception = Assert.Throws<InvalidStateException>(() => sprint.AddTeamMember(dev2));
            Assert.Contains("Cannot add team members", exception.Message);
        }

        [Fact]
        // Requirement: FR04 (regel: backlog items toevoegen voor start)
        public void AddBacklogItem_Voegt_Toe_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var item = CreateBacklogItem();
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act
            sprint.AddBacklogItem(item);

            // Assert
            Assert.Contains(item, sprint.SprintBacklog.Items);
        }

        [Fact]
        // Requirement: FR04 (regel: backlog items niet toevoegen na start)
        public void AddBacklogItem_Gooit_Exception_In_RunningState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint(); // Naar Running
            Assert.IsType<RunningState>(sprint.CurrentState);
            var newItem = CreateBacklogItem("Nieuw Item");

            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.AddBacklogItem(newItem));
        }


        [Fact]
        // Requirement: FR04 (regel: sprint properties wijzigen voor start)
        public void ChangeName_Werkt_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var newName = "Nieuwe Sprint Naam";
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act
            sprint.ChangeName(newName);

            // Assert
            Assert.Equal(newName, sprint.Name);
        }

        [Fact]
        // Requirement: FR04 (regel: sprint properties niet wijzigen na start)
        public void ChangeName_Gooit_Exception_In_RunningState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint(); // Naar Running
            Assert.IsType<RunningState>(sprint.CurrentState);
            var newName = "Nieuwe Sprint Naam";

            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.ChangeName(newName));
        }

        [Fact]
        // Requirement: FR04 (regel: sprint properties wijzigen voor start)
        public void ChangeDates_Werkt_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var newStart = DateTime.Now.AddDays(1).Date;
            var newEnd = DateTime.Now.AddDays(15).Date;
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act
            sprint.ChangeDates(newStart, newEnd);

            // Assert
            Assert.Equal(newStart, sprint.StartDate.Date);
            Assert.Equal(newEnd, sprint.EndDate.Date);
        }

        [Fact]
        // Requirement: FR04 (regel: sprint properties niet wijzigen na start)
        public void ChangeDates_Gooit_Exception_In_RunningState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint(); // Naar Running
            Assert.IsType<RunningState>(sprint.CurrentState);
            var newStart = DateTime.Now.AddDays(1);
            var newEnd = DateTime.Now.AddDays(15);

            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.ChangeDates(newStart, newEnd));
        }

        [Fact]
        // Requirement: FR04 (use case: sprint starten)
        public void StartSprint_Verandert_State_Naar_Running_Als_Voorwaarden_Voldaan()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var dev = CreateDeveloper();
            var item = CreateBacklogItem();
            sprint.AddTeamMember(dev);
            sprint.AddBacklogItem(item);
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act
            sprint.StartSprint();

            // Assert
            Assert.IsType<RunningState>(sprint.CurrentState);
        }

        [Fact]
        // Requirement: FR04 (regel: team nodig voor start)
        public void StartSprint_Gooit_Exception_Zonder_Teamleden()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var item = CreateBacklogItem();
            sprint.AddBacklogItem(item); // Geen teamleden toegevoegd
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartSprint());
            Assert.Contains("No team members assigned", exception.Message);
        }

        [Fact]
        // Requirement: FR04 (regel: items nodig voor start)
        public void StartSprint_Gooit_Exception_Zonder_BacklogItems()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var dev = CreateDeveloper();
            sprint.AddTeamMember(dev); // Geen items toegevoegd
            Assert.IsType<CreatedState>(sprint.CurrentState);

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartSprint());
            Assert.Contains("No backlog items in the sprint backlog", exception.Message);
        }


        [Fact]
        // Requirement: FR04 (use case: sprint afronden)
        public void FinishSprint_Verandert_State_Van_Running_Naar_Finished()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint(); // Naar Running
            Assert.IsType<RunningState>(sprint.CurrentState);

            // Act
            sprint.FinishSprint();

            // Assert
            Assert.IsType<FinishedState>(sprint.CurrentState);
        }

        [Fact]
        // Requirement: FR08 (use case: sprint reviewen)
        public void ReviewSprint_Verandert_State_Van_Finished_Naar_Reviewed_Voor_ReviewSprint()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            // Simuleer flow naar Finished
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint(); // Naar Finished
            Assert.IsType<FinishedState>(sprint.CurrentState);
            string docPath = "pad/naar/review.docx";

            // Act
            sprint.ReviewSprint(docPath);

            // Assert
            Assert.IsType<ReviewedState>(sprint.CurrentState);
            Assert.Equal(docPath, sprint.ReviewSummaryDocumentPath);
        }

        [Fact]
        // Requirement: FR08 (regel: alleen Review sprints reviewen)
        public void ReviewSprint_Gooit_Exception_Voor_ReleaseSprint()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            // Simuleer flow naar Finished
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint(); // Naar Finished
            Assert.IsType<FinishedState>(sprint.CurrentState);

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.ReviewSprint("pad/naar/review.docx"));
            Assert.Contains("not a Review sprint", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (use case: release starten)
        // Path Coverage: Test het succespad van StartRelease
        public void StartRelease_Verandert_State_Van_Finished_Naar_Releasing_Voor_ReleaseSprint()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint(); // Heeft nu een dummy pipeline
                                                   // Simuleer flow naar Finished
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint(); // Naar Finished
            Assert.IsType<FinishedState>(sprint.CurrentState);
            Action<bool> dummyCallback = (success) => { }; // Callback voor state

            // Act
            sprint.StartRelease(dummyCallback);

            // Assert
            Assert.IsType<ReleasingState>(sprint.CurrentState);
        }

        [Fact]
        // Requirement: FR07 (regel: alleen Release sprints releasen)
        // Path Coverage: Test het pad waar type niet Release is
        public void StartRelease_Gooit_Exception_Voor_ReviewSprint()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            // Simuleer flow naar Finished
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint(); // Naar Finished
            Assert.IsType<FinishedState>(sprint.CurrentState);
            Action<bool> dummyCallback = (success) => { };

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartRelease(dummyCallback));
            Assert.Contains("not a Release sprint", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (regel: pipeline nodig voor release)
        // Path Coverage: Test het pad waar pipeline null is
        public void StartRelease_Gooit_Exception_Zonder_Pipeline()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint(withPipeline: false); // Geen pipeline
                                                                      // Simuleer flow naar Finished
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint(); // Naar Finished
            Assert.IsType<FinishedState>(sprint.CurrentState);
            Action<bool> dummyCallback = (success) => { };

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartRelease(dummyCallback));
            Assert.Contains("No pipeline associated", exception.Message);
        }


        [Fact]
        // Requirement: FR07 (use case: release annuleren)
        public void CancelRelease_Verandert_State_Van_Releasing_Naar_Cancelled()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            // Simuleer flow naar Releasing
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint();
            Action<bool> dummyCallback = (success) => { };
            sprint.StartRelease(dummyCallback); // Naar Releasing
            Assert.IsType<ReleasingState>(sprint.CurrentState);

            // Act
            sprint.CancelRelease();

            // Assert
            Assert.IsType<CancelledState>(sprint.CurrentState);
        }

        [Fact]
        // Requirement: FR07 & FR06 (Observer test voor annulering)
        public void CancelRelease_Notificeert_Observers()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            var mockObserver = new Mock<IObserver>();
            sprint.Attach(mockObserver.Object); // Voeg extra observer toe

            // Simuleer flow naar Releasing
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint();
            Action<bool> dummyCallback = (success) => { };
            sprint.StartRelease(dummyCallback); // Naar Releasing

            // Act
            sprint.CancelRelease(); // Zou moeten notificeren

            // Assert
            // PO en SM zijn al observers. Controleer of de mock ook is aangeroepen.
            // Het bericht bevat idealiter "cancelled".
            mockObserver.Verify(o => o.Update(sprint, It.Is<string>(s => s.Contains("cancelled", StringComparison.OrdinalIgnoreCase))), Times.AtLeastOnce);
            // We checken AtLeastOnce omdat SetState ook notificaties kan sturen.
        }


        // --- Tests voor HandleReleaseResult (via callback simulatie) ---
        // Deze tests simuleren de callback die normaal door de PipelineExecutor wordt aangeroepen.
        // Path Coverage: Deze tests dekken de twee hoofdpaden (succes/falen) van HandleReleaseResult.

        [Fact]
        // Requirement: FR07 (use case: pipeline succesvol)
        public void HandleReleaseResult_True_Verandert_State_Van_Releasing_Naar_Released()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            bool callbackResult = false; // Houd bij of callback is aangeroepen
            Action<bool> releaseCallback = (success) => { callbackResult = success; };

            // Simuleer flow naar Releasing
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint();
            sprint.StartRelease(releaseCallback); // Naar Releasing
            var releasingState = sprint.CurrentState as ReleasingState;
            Assert.NotNull(releasingState); // Zorg dat we in de juiste state zijn

            // Act
            // Simuleer de callback van de executor met succes = true
            releasingState.HandleReleaseResult(true);

            // Assert
            Assert.IsType<ReleasedState>(sprint.CurrentState);
            Assert.True(callbackResult); // Controleer of de originele callback is aangeroepen met true
        }

        [Fact]
        // Requirement: FR07 (use case: pipeline faalt)
        public void HandleReleaseResult_False_Verandert_State_Van_Releasing_Naar_Finished()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            bool callbackResult = true; // Initialiseer om verandering te zien
            Action<bool> releaseCallback = (success) => { callbackResult = success; };

            // Simuleer flow naar Releasing
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            sprint.StartSprint();
            sprint.FinishSprint();
            sprint.StartRelease(releaseCallback); // Naar Releasing
            var releasingState = sprint.CurrentState as ReleasingState;
            Assert.NotNull(releasingState);

            // Act
            // Simuleer de callback van de executor met succes = false
            releasingState.HandleReleaseResult(false);

            // Assert
            Assert.IsType<FinishedState>(sprint.CurrentState); // Terug naar Finished
            Assert.False(callbackResult); // Controleer of de originele callback is aangeroepen met false
        }

        // --- Tests voor CloseSprint ---

        [Fact]
        // Test voor AC-FR08.4: Given een Sprint in 'Released' state / When CloseSprint wordt aangeroepen / Then de actie slaagt (geen exception).
        public void Test_AC_FR08_4_CloseSprint_Valid_From_Released()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.SetState(new ReleasedState(sprint)); // Forceer state
            Assert.IsType<ReleasedState>(sprint.CurrentState);

            // Act
            var exception = Record.Exception(() => sprint.CloseSprint()); // Vang eventuele exception op

            // Assert
            Assert.Null(exception); // Er mag geen exception zijn
        }

        [Fact]
        // Test voor AC-FR08.5: Given een Sprint in 'Reviewed' state / When CloseSprint wordt aangeroepen / Then de actie slaagt (geen exception).
        public void Test_AC_FR08_5_CloseSprint_Valid_From_Reviewed()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.SetState(new ReviewedState(sprint)); // Forceer state
            Assert.IsType<ReviewedState>(sprint.CurrentState);

            // Act
            var exception = Record.Exception(() => sprint.CloseSprint());

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        // Test voor AC-FR08.6: Given een Sprint in 'Cancelled' state / When CloseSprint wordt aangeroepen / Then de actie slaagt (geen exception).
        public void Test_AC_FR08_6_CloseSprint_Valid_From_Cancelled()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.SetState(new CancelledState(sprint)); // Forceer state
            Assert.IsType<CancelledState>(sprint.CurrentState);

            // Act
            var exception = Record.Exception(() => sprint.CloseSprint());

            // Assert
            Assert.Null(exception);
        }

        // --- GECORRIGEERDE TEST HIERONDER ---
        [Theory]
        [InlineData("Created")]
        [InlineData("Running")]
        [InlineData("Finished")]
        [InlineData("Releasing")]
        // Test voor AC-FR08.7 - AC-FR08.10: Given een Sprint in [State] state / When CloseSprint wordt aangeroepen / Then een InvalidStateException wordt geworpen.
        public void Test_AC_FR08_7_To_10_CloseSprint_Invalid_From_Intermediate_States(string stateName)
        {
            // Arrange
            var sprint = CreateNewReleaseSprint(); // Start als Release voor Releasing state
            Type expectedStateType = null; // Variabele voor het verwachte Type object

            // Zet de sprint in de gewenste beginstaat EN sla het verwachte Type op
            switch (stateName)
            {
                case "Created":
                    expectedStateType = typeof(CreatedState);
                    // Is al in Created state
                    break;
                case "Running":
                    expectedStateType = typeof(RunningState);
                    sprint.AddTeamMember(CreateDeveloper());
                    sprint.AddBacklogItem(CreateBacklogItem());
                    sprint.StartSprint();
                    break;
                case "Finished":
                    expectedStateType = typeof(FinishedState);
                    sprint.AddTeamMember(CreateDeveloper());
                    sprint.AddBacklogItem(CreateBacklogItem());
                    sprint.StartSprint();
                    sprint.FinishSprint();
                    break;
                case "Releasing":
                    expectedStateType = typeof(ReleasingState);
                    sprint.AddTeamMember(CreateDeveloper());
                    sprint.AddBacklogItem(CreateBacklogItem());
                    sprint.StartSprint();
                    sprint.FinishSprint();
                    sprint.StartRelease((_) => { }); // Zet naar Releasing
                    break;
            }

            Assert.NotNull(expectedStateType); // Controleer of type is ingesteld
            Assert.IsType(expectedStateType, sprint.CurrentState); // Verifieer beginstaat

            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.CloseSprint());
            Assert.IsType(expectedStateType, sprint.CurrentState); // State blijft ongewijzigd
        }
        
    }
}
