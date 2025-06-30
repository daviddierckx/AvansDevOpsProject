using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.States.BacklogItemStates;
using AvansDevOps.App.Domain.Exceptions;
using AvansDevOps.App.Domain.Interfaces.Patterns;
using Moq;
using System;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class BacklogItemTests
    {
        private BacklogItem CreateNewBacklogItem(string title = "Test Item") => new BacklogItem(title, "Beschrijving", 5);
        private Developer CreateDeveloper(string name = "Test Dev") => new Developer(name, name + "@test.com");
        private Activity CreateActivity(string desc = "Test Activity") => new Activity(desc);

        [Fact]
        public void Test_AC_FR03_1_NewBacklogItem_Has_TodoState()
        {
            var item = CreateNewBacklogItem();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_1_StartTask_Valid_From_Todo_To_Doing()
        {
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            Assert.IsType<DoingState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_3_MarkAsReadyForTesting_Valid_From_Doing_To_ReadyForTesting()
        {
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            Assert.IsType<ReadyForTestingState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_4_StartTesting_Valid_From_ReadyForTesting_To_Testing()
        {
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            Assert.IsType<TestingState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_6_SendTestingResult_False_From_Testing_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            item.SendTestingResult(false);
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_5_SendTestingResult_True_From_Testing_To_Tested()
        {
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            item.SendTestingResult(true);
            Assert.IsType<TestedState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_7_CompleteTask_Valid_From_Tested_To_Done()
        {
            var item = CreateNewBacklogItem();
            var activity = CreateActivity();
            item.AddActivity(activity);
            item.AssignedDeveloper = CreateDeveloper();
            item.SetState(new TestedState(item));
            activity.MarkAsDone();
            item.CompleteTask();
            Assert.IsType<DoneState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_9_ReopenTask_From_Done_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new DoneState(item));
            item.ReopenTask();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_10_ReopenTask_From_Tested_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new TestedState(item));
            item.ReopenTask();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_11_ReopenTask_From_Testing_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new TestingState(item));
            item.ReopenTask();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_12_ReopenTask_From_ReadyForTesting_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new ReadyForTestingState(item));
            item.ReopenTask();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_13_ReopenTask_From_Doing_To_Todo()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new DoingState(item));
            item.ReopenTask();
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_2_StartTask_Invalid_Without_Developer()
        {
            var item = CreateNewBacklogItem();
            var exception = Assert.Throws<InvalidStateException>(() => item.StartTask());
            Assert.Contains("No developer assigned", exception.Message);
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR05_8_CompleteTask_Invalid_With_Incomplete_Activities()
        {
            var item = CreateNewBacklogItem();
            item.AddActivity(CreateActivity());
            item.SetState(new TestedState(item));
            var exception = Assert.Throws<InvalidOperationException>(() => item.CompleteTask());
            Assert.Contains("Not all its activities are marked as done", exception.Message);
            Assert.IsType<TestedState>(item.CurrentState);
        }

        [Theory]
        [InlineData("StartTask")]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        public void Test_AC_FR05_19_Invalid_Actions_From_DoneState(string action)
        {
            var item = CreateNewBacklogItem();
            item.SetState(new DoneState(item));
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<DoneState>(item.CurrentState);
        }

        [Theory]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        public void Test_AC_FR05_14_Invalid_Actions_From_TodoState(string action)
        {
            var item = CreateNewBacklogItem();
            switch (action)
            {
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Theory]
        [InlineData("StartTask")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        public void Test_AC_FR05_15_Invalid_Actions_From_DoingState(string action)
        {
            var item = CreateNewBacklogItem();
            item.SetState(new DoingState(item));
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<DoingState>(item.CurrentState);
        }

        [Theory]
        [InlineData("StartTask")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        public void Test_AC_FR05_16_Invalid_Actions_From_ReadyForTestingState(string action)
        {
            var item = CreateNewBacklogItem();
            item.SetState(new ReadyForTestingState(item));
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<ReadyForTestingState>(item.CurrentState);
        }

        [Fact]
        public void Test_FR05_Calling_MarkAsReadyForTesting_When_Already_ReadyForTesting_Throws_No_Exception()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.SetState(new ReadyForTestingState(item));

            // Act
            var exception = Record.Exception(() => item.MarkAsReadyForTesting());

            // Assert
            Assert.Null(exception);
            Assert.IsType<ReadyForTestingState>(item.CurrentState);
        }


        [Theory]
        [InlineData("StartTask")]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("CompleteTask")]
        public void Test_AC_FR05_17_Invalid_Actions_From_TestingState(string action)
        {
            var item = CreateNewBacklogItem();
            item.SetState(new TestingState(item));
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<TestingState>(item.CurrentState);
        }

        [Fact]
        public void Test_FR05_Calling_StartTesting_When_Already_Testing_Throws_No_Exception()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.SetState(new TestingState(item));

            // Act
            var exception = Record.Exception(() => item.StartTesting());

            // Assert
            Assert.Null(exception);
            Assert.IsType<TestingState>(item.CurrentState);
        }

        [Theory]
        [InlineData("StartTask")]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        public void Test_AC_FR05_18_Invalid_Actions_From_TestedState(string action)
        {
            var item = CreateNewBacklogItem();
            item.SetState(new TestedState(item));
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(false)); break;
            }
            Assert.IsType<TestedState>(item.CurrentState);
        }

        [Fact]
        public void Test_AC_FR06_7_AddDiscussionThread_Invalid_When_Item_Is_Done()
        {
            var item = CreateNewBacklogItem();
            item.SetState(new DoneState(item));
            var thread = new DiscussionThread("Test discussie", item);
            var exception = Assert.Throws<InvalidOperationException>(() => item.AddDiscussionThread(thread));
            Assert.Contains("Cannot add discussion threads to a completed backlog item", exception.Message);
        }

        [Fact]
        public void Test_AC_FR06_7_AddMessage_Invalid_When_Item_Is_Done()
        {
            var item = CreateNewBacklogItem();
            var thread = new DiscussionThread("Test discussie", item);
            item.AddDiscussionThread(thread);
            item.SetState(new DoneState(item));
            var message = new Message("Nieuw bericht", CreateDeveloper());
            var exception = Assert.Throws<InvalidOperationException>(() => thread.AddMessage(message));
            Assert.Contains("Cannot add messages to a discussion for a completed backlog item", exception.Message);
        }

        [Fact]
        public void Test_AC_FR06_2_SendTestingResult_False_Notifies_Observer()
        {
            var item = CreateNewBacklogItem();
            var dev = CreateDeveloper();
            item.AssignedDeveloper = dev;
            var mockObserver = new Mock<IObserver>();
            item.Attach(mockObserver.Object);
            item.SetState(new TestingState(item));
            item.SendTestingResult(false);
            mockObserver.Verify(o => o.Update(item, It.Is<string>(s => s.Contains("FAILED"))), Times.Once);
        }

        [Fact]
        public void Test_FR06_Detach_Observer_Prevents_Notification()
        {
            var item = CreateNewBacklogItem();
            var mockObserver = new Mock<IObserver>();
            item.Attach(mockObserver.Object);
            item.Detach(mockObserver.Object);
            item.SetState(new TestingState(item));
            item.SendTestingResult(false);
            mockObserver.Verify(o => o.Update(It.IsAny<ISubject>(), It.IsAny<string>()), Times.Never);
        }
    }
}