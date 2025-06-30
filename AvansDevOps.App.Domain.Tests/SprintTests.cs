using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Exceptions;
using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.Interfaces.States;
using AvansDevOps.App.Domain.States.SprintStates;
using Moq;
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
            var sprint = new Sprint("Review Sprint", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), SprintType.Review, sm, project);
            return sprint;
        }

        private Sprint CreateNewReleaseSprint(bool withPipeline = true)
        {
            var sm = CreateScrumMaster();
            var project = CreateProject();
            var sprint = new Sprint("Release Sprint", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(10), SprintType.Release, sm, project);
            if (withPipeline)
            {
                sprint.Pipeline = new DevelopmentPipeline("Dummy Pipeline");
                sprint.Pipeline.AddAction(new BuildAction("Dummy Build"));
            }
            return sprint;
        }

        // --- Tests ---

        [Fact]
        public void Test_AC_FR04_1_NewSprint_Has_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            // Assert
            Assert.IsType<CreatedState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_AC_FR04_2_AddTeamMember_In_CreatedState()
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
        public void Test_AC_FR04_4_RemoveTeamMember_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var dev = CreateDeveloper();
            sprint.AddTeamMember(dev);
            Assert.Contains(dev, sprint.TeamMembers);

            // Act
            sprint.RemoveTeamMember(dev);

            // Assert
            Assert.DoesNotContain(dev, sprint.TeamMembers);
        }

        [Fact]
        public void Test_AC_FR04_6_AddBacklogItem_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var item = CreateBacklogItem();
            // Act
            sprint.AddBacklogItem(item);
            // Assert
            Assert.Contains(item, sprint.SprintBacklog.Items);
        }

        [Fact]
        public void Test_AC_FR04_8_RemoveBacklogItem_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var item = CreateBacklogItem();
            sprint.AddBacklogItem(item);
            Assert.Contains(item, sprint.SprintBacklog.Items);

            // Act
            sprint.RemoveBacklogItem(item);

            // Assert
            Assert.DoesNotContain(item, sprint.SprintBacklog.Items);
        }

        [Fact]
        public void Test_AC_FR04_10_ChangeName_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var newName = "Nieuwe Sprint Naam";
            // Act
            sprint.ChangeName(newName);
            // Assert
            Assert.Equal(newName, sprint.Name);
        }

        [Fact]
        public void Test_AC_FR04_12_ChangeDates_In_CreatedState()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            var newStart = DateTime.Now.AddDays(1).Date;
            var newEnd = DateTime.Now.AddDays(15).Date;
            // Act
            sprint.ChangeDates(newStart, newEnd);
            // Assert
            Assert.Equal(newStart, sprint.StartDate.Date);
            Assert.Equal(newEnd, sprint.EndDate.Date);
        }

        [Fact]
        public void Test_AC_FR04_15_StartSprint_Valid()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            sprint.AddBacklogItem(CreateBacklogItem());
            // Act
            sprint.StartSprint();
            // Assert
            Assert.IsType<RunningState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_AC_FR04_16_StartSprint_ThrowsException_Without_TeamMembers()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddBacklogItem(CreateBacklogItem());
            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartSprint());
            Assert.Contains("No team members assigned", exception.Message);
        }

        [Fact]
        public void Test_AC_FR04_17_StartSprint_ThrowsException_Without_BacklogItems()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.AddTeamMember(CreateDeveloper());
            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => sprint.StartSprint());
            Assert.Contains("No backlog items in the sprint backlog", exception.Message);
        }

        [Fact]
        public void Test_AC_FR04_18_FinishSprint_From_Running_To_Finished()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.SetState(new RunningState(sprint));
            // Act
            sprint.FinishSprint();
            // Assert
            Assert.IsType<FinishedState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_AC_FR08_1_ReviewSprint_From_Finished_To_Reviewed()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.SetState(new FinishedState(sprint));
            string docPath = "pad/naar/review.docx";
            // Act
            sprint.ReviewSprint(docPath);
            // Assert
            Assert.IsType<ReviewedState>(sprint.CurrentState);
            Assert.Equal(docPath, sprint.ReviewSummaryDocumentPath);
        }

        [Fact]
        public void Test_AC_FR08_3_ReviewSprint_ThrowsException_With_Empty_DocumentPath()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.SetState(new FinishedState(sprint));
            // Act & Assert
            Assert.Throws<ArgumentException>(() => sprint.ReviewSprint(null));
        }

        [Fact]
        public void Test_AC_FR07_2_StartRelease_From_Finished_To_Releasing()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.SetState(new FinishedState(sprint));
            // Act
            sprint.StartRelease((_) => { });
            // Assert
            Assert.IsType<ReleasingState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_FR07_StartRelease_Works_With_Empty_Pipeline()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.Pipeline = new DevelopmentPipeline("Lege Pipeline");
            sprint.SetState(new FinishedState(sprint));

            // Act
            sprint.StartRelease((_) => { });

            // Assert
            Assert.IsType<ReleasingState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_AC_FR06_3_CancelRelease_To_Cancelled_And_Notifies()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            var mockObserver = new Mock<IObserver>();
            sprint.Attach(mockObserver.Object);
            sprint.SetState(new ReleasingState(sprint, (_) => { }));
            // Act
            sprint.CancelRelease();
            // Assert
            Assert.IsType<CancelledState>(sprint.CurrentState);
            mockObserver.Verify(o => o.Update(sprint, It.Is<string>(s => s.Contains("cancelled", StringComparison.OrdinalIgnoreCase))), Times.AtLeastOnce);
        }

        [Fact]
        public void Test_AC_FR07_5_HandleReleaseResult_True_To_Released()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.SetState(new ReleasingState(sprint, (_) => { }));
            var releasingState = sprint.CurrentState as ReleasingState;
            // Act
            releasingState.HandleReleaseResult(true);
            // Assert
            Assert.IsType<ReleasedState>(sprint.CurrentState);
        }

        [Fact]
        public void Test_AC_FR07_6_HandleReleaseResult_False_To_Finished_And_Notifies()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            var mockObserver = new Mock<IObserver>();
            sprint.Attach(mockObserver.Object);
            sprint.SetState(new ReleasingState(sprint, (_) => { }));
            var releasingState = sprint.CurrentState as ReleasingState;
            // Act
            releasingState.HandleReleaseResult(false);
            // Assert
            Assert.IsType<FinishedState>(sprint.CurrentState);
            mockObserver.Verify(o => o.Update(sprint, It.Is<string>(s => s.Contains("failed", StringComparison.OrdinalIgnoreCase))), Times.Once);
        }

        [Theory]
        [InlineData("Running")]
        [InlineData("Finished")]
        [InlineData("Releasing")]
        [InlineData("Reviewed")]
        [InlineData("Released")]
        [InlineData("Cancelled")]
        public void Test_AC_FR04_11_ChangeName_Invalid_In_NonCreatedStates(string stateName)
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            SetSprintStateByName(sprint, stateName);
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.ChangeName("Nieuwe naam"));
        }

        [Theory]
        [InlineData("Running")]
        [InlineData("Finished")]
        [InlineData("Releasing")]
        [InlineData("Reviewed")]
        [InlineData("Released")]
        [InlineData("Cancelled")]
        public void Test_AC_FR04_3_AddTeamMember_Invalid_In_NonCreatedStates(string stateName)
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            SetSprintStateByName(sprint, stateName);
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.AddTeamMember(CreateDeveloper()));
        }

        [Fact]
        public void Test_AC_FR08_2_ReviewSprint_Invalid_For_ReleaseSprint()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint();
            sprint.SetState(new FinishedState(sprint));
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.ReviewSprint("doc.path"));
        }

        [Fact]
        public void Test_AC_FR07_4_StartRelease_Invalid_For_ReviewSprint()
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            sprint.SetState(new FinishedState(sprint));
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.StartRelease((_) => { }));
        }


        [Fact]
        public void Test_AC_FR07_3_StartRelease_Invalid_Without_Pipeline()
        {
            // Arrange
            var sprint = CreateNewReleaseSprint(withPipeline: false);
            sprint.SetState(new FinishedState(sprint));
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.StartRelease((_) => { }));
        }


        [Theory]
        [InlineData("Created")]
        [InlineData("Running")]
        [InlineData("Finished")]
        [InlineData("Releasing")]
        public void Test_AC_FR08_7_to_10_CloseSprint_Invalid_From_Intermediate_States(string stateName)
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            SetSprintStateByName(sprint, stateName);
            // Act & Assert
            Assert.Throws<InvalidStateException>(() => sprint.CloseSprint());
        }


        [Theory]
        [InlineData("Reviewed")]
        [InlineData("Released")]
        [InlineData("Cancelled")]
        public void Test_AC_FR08_4_to_6_CloseSprint_Valid_From_Final_States(string stateName)
        {
            // Arrange
            var sprint = CreateNewReviewSprint();
            SetSprintStateByName(sprint, stateName);
            // Act
            var exception = Record.Exception(() => sprint.CloseSprint());
            // Assert
            Assert.Null(exception);
        }

        // --- Hulpfunctie om state in te stellen op basis van string ---
        private void SetSprintStateByName(Sprint sprint, string stateName)
        {
            Action<bool> dummyCallback = (_) => { };
            ISprintState state = stateName switch
            {
                "Created" => new CreatedState(sprint),
                "Running" => new RunningState(sprint),
                "Finished" => new FinishedState(sprint),
                "Releasing" => new ReleasingState(sprint, dummyCallback),
                "Reviewed" => new ReviewedState(sprint),
                "Released" => new ReleasedState(sprint),
                "Cancelled" => new CancelledState(sprint),
                _ => throw new ArgumentException("Invalid state name for test setup")
            };
            sprint.SetState(state);
            Assert.IsType(state.GetType(), sprint.CurrentState); // Verifieer dat de state correct is gezet
        }
    }
}