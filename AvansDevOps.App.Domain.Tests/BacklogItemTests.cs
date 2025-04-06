

using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.States.BacklogItemStates; // Voor state check
using AvansDevOps.App.Domain.Exceptions;
using AvansDevOps.App.Domain.Interfaces.Patterns; // Voor IObserver/ISubject
using Moq; // Voor mocking
using System;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class BacklogItemTests
    {
        // --- Hulpfuncties ---
        private BacklogItem CreateNewBacklogItem(string title = "Test Item")
        {
            return new BacklogItem(title, "Beschrijving", 5);
        }

        private Developer CreateDeveloper(string name = "Test Dev")
        {
            return new Developer(name, name + "@test.com");
        }

        private Activity CreateActivity(string desc = "Test Activity")
        {
            return new Activity(desc);
        }

        // --- State Initialization Test ---

        [Fact]
        // Requirement: FR03 (impliciet: item start in Todo)
        public void Nieuw_BacklogItem_Heeft_TodoState()
        {
            // Arrange
            var item = CreateNewBacklogItem();

            // Act
            var currentState = item.CurrentState;

            // Assert
            Assert.IsType<TodoState>(currentState);
        }

        // --- Valid State Transition Tests ---

        [Fact]
        // Requirement: FR05 (use case: taak starten)
        public void StartTask_Met_Developer_Verandert_State_Van_Todo_Naar_Doing()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var dev = CreateDeveloper();
            item.AssignedDeveloper = dev; // Wijs developer toe

            // Act
            item.StartTask();

            // Assert
            Assert.IsType<DoingState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak klaar voor test)
        public void MarkAsReadyForTesting_Verandert_State_Van_Doing_Naar_ReadyForTesting()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask(); // Naar DoingState

            // Act
            item.MarkAsReadyForTesting();

            // Assert
            Assert.IsType<ReadyForTestingState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: starten met testen)
        public void StartTesting_Verandert_State_Van_ReadyForTesting_Naar_Testing()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting(); // Naar ReadyForTestingState

            // Act
            item.StartTesting();

            // Assert
            Assert.IsType<TestingState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: test faalt)
        public void SendTestingResult_False_Verandert_State_Van_Testing_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting(); // Naar TestingState

            // Act
            item.SendTestingResult(false); // Test faalt

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: test slaagt)
        public void SendTestingResult_True_Verandert_State_Van_Testing_Naar_Tested()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting(); // Naar TestingState

            // Act
            item.SendTestingResult(true); // Test slaagt

            // Assert
            Assert.IsType<TestedState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak afronden)
        // Requirement: FR03 (regel: alle activities klaar)
        public void CompleteTask_Verandert_State_Van_Tested_Naar_Done_Als_Alle_Activiteiten_Klaar_Zijn()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var activity1 = CreateActivity("Doe dit");
            var activity2 = CreateActivity("Doe dat");
            item.AddActivity(activity1);
            item.AddActivity(activity2);

            // Simuleer de flow naar Tested state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            item.SendTestingResult(true); // Naar TestedState

            // Markeer activiteiten als klaar
            activity1.MarkAsDone();
            activity2.MarkAsDone();

            // Act
            item.CompleteTask();

            // Assert
            Assert.IsType<DoneState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak heropenen)
        public void ReopenTask_Verandert_State_Van_Done_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            // Simuleer de flow naar Done state (vereenvoudigd, zonder activiteiten hier)
            item.SetState(new DoneState(item)); // Forceer state voor deze test

            // Act
            item.ReopenTask();

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak heropenen vanuit Tested)
        public void ReopenTask_Verandert_State_Van_Tested_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            // Simuleer flow naar Tested state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            item.SendTestingResult(true); // Naar TestedState

            // Act
            item.ReopenTask();

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak heropenen vanuit Testing)
        public void ReopenTask_Verandert_State_Van_Testing_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            // Simuleer flow naar Testing state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting(); // Naar TestingState

            // Act
            item.ReopenTask();

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak heropenen vanuit ReadyForTesting)
        public void ReopenTask_Verandert_State_Van_ReadyForTesting_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            // Simuleer flow naar ReadyForTesting state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting(); // Naar ReadyForTestingState

            // Act
            item.ReopenTask();

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        [Fact]
        // Requirement: FR05 (use case: taak heropenen vanuit Doing)
        public void ReopenTask_Verandert_State_Van_Doing_Naar_Todo()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            // Simuleer flow naar Doing state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask(); // Naar DoingState

            // Act
            item.ReopenTask();

            // Assert
            Assert.IsType<TodoState>(item.CurrentState);
        }

        // --- Invalid State Transition / Business Rule Tests ---

        [Fact]
        // Requirement: FR05 (regel: developer nodig om te starten)
        public void StartTask_Zonder_Developer_Gooit_Exception()
        {
            // Arrange
            var item = CreateNewBacklogItem(); // Geen developer toegewezen

            // Act & Assert
            var exception = Assert.Throws<InvalidStateException>(() => item.StartTask());
            Assert.Contains("No developer assigned", exception.Message); // Controleer foutmelding
            Assert.IsType<TodoState>(item.CurrentState); // State mag niet veranderen
        }

        [Fact]
        // Requirement: FR03 (regel: alle activities klaar voor Done)
        public void CompleteTask_Gooit_Exception_Van_Tested_Als_Niet_Alle_Activiteiten_Klaar_Zijn()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var activity1 = CreateActivity("Doe dit");
            var activity2 = CreateActivity("Doe dat"); // Deze blijft open
            item.AddActivity(activity1);
            item.AddActivity(activity2);

            // Simuleer de flow naar Tested state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();
            item.SendTestingResult(true); // Naar TestedState

            // Markeer alleen de eerste activiteit als klaar
            activity1.MarkAsDone();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => item.CompleteTask());
            Assert.Contains("Not all its activities are marked as done", exception.Message); // Controleer de message.
            Assert.IsType<TestedState>(item.CurrentState); // State blijft Tested
        }

        [Theory] // Gebruik Theory om meerdere ongeldige acties vanuit een state te testen
        [InlineData("StartTask")]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        // Requirement: FR05 (regel: Done state is eindstatus, behalve Reopen)
        public void Ongeldige_Acties_Vanuit_DoneState_Gooien_Exception(string action)
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.SetState(new DoneState(item)); // Forceer state

            // Act & Assert
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break; // Of geen exception maar melding? Huidige code gooit exception.
            }
            Assert.IsType<DoneState>(item.CurrentState); // State blijft Done
        }

        [Theory]
        [InlineData("MarkAsReadyForTesting")]
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        // Requirement: FR05 (regel: ongeldige transities vanuit Todo)
        public void Ongeldige_Acties_Vanuit_TodoState_Gooien_Exception(string action)
        {
            // Arrange
            var item = CreateNewBacklogItem(); // Start in TodoState

            // Act & Assert
            switch (action)
            {
                case "MarkAsReadyForTesting": Assert.Throws<InvalidStateException>(() => item.MarkAsReadyForTesting()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<TodoState>(item.CurrentState); // State blijft Todo
        }

        [Theory]
        [InlineData("StartTask")] // Kan niet opnieuw gestart worden
        [InlineData("StartTesting")]
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        // Requirement: FR05 (regel: ongeldige transities vanuit Doing)
        public void Ongeldige_Acties_Vanuit_DoingState_Gooien_Exception(string action)
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask(); // Naar DoingState

            // Act & Assert
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "StartTesting": Assert.Throws<InvalidStateException>(() => item.StartTesting()); break;
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            Assert.IsType<DoingState>(item.CurrentState); // State blijft Doing
        }

        [Theory]
        [InlineData("StartTask")]
        [InlineData("MarkAsReadyForTesting")] // Al voorbij deze stap
        [InlineData("SendTestingResult")]
        [InlineData("CompleteTask")]
        // Requirement: FR05 (regel: ongeldige transities vanuit ReadyForTesting)
        public void Ongeldige_Acties_Vanuit_ReadyForTestingState_Gooien_Exception(string action)
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting(); // Naar ReadyForTestingState

            // Act & Assert
            switch (action)
            {
                case "StartTask": Assert.Throws<InvalidStateException>(() => item.StartTask()); break;
                case "MarkAsReadyForTesting": item.MarkAsReadyForTesting(); break; // Geen exception, alleen melding (test apart)
                case "SendTestingResult": Assert.Throws<InvalidStateException>(() => item.SendTestingResult(true)); break;
                case "CompleteTask": Assert.Throws<InvalidStateException>(() => item.CompleteTask()); break;
            }
            if (action != "MarkAsReadyForTesting") // State blijft gelijk behalve bij dubbel aanroepen MarkAs...
                Assert.IsType<ReadyForTestingState>(item.CurrentState);
        }

        // Voeg vergelijkbare Theory tests toe voor TestingState en TestedState

        [Fact]
        // Requirement: FR06 (regel: geen discussie bij Done item)
        public void AddDiscussionThread_Gooit_Exception_Als_Item_Done_Is()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            item.SetState(new DoneState(item)); // Forceer state
            var user = CreateDeveloper();
            var thread = new DiscussionThread("Test discussie", item);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => item.AddDiscussionThread(thread));
            Assert.Contains("Cannot add discussion threads to a completed backlog item", exception.Message);
        }

        [Fact]
        // Requirement: FR06 (regel: geen discussie bij Done item - via Message)
        public void AddMessage_To_Thread_Gooit_Exception_Als_Item_Done_Is()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var user = CreateDeveloper();
            var thread = new DiscussionThread("Test discussie", item);
            item.AddDiscussionThread(thread); // Toevoegen als item nog niet Done is

            item.SetState(new DoneState(item)); // Zet state naar Done *na* toevoegen thread

            var message = new Message("Nieuw bericht", user);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => thread.AddMessage(message));
            Assert.Contains("Cannot add messages to a discussion for a completed backlog item", exception.Message);
        }


        // --- Observer Pattern Test ---

        [Fact]
        // Requirement: FR06 (Observer pattern test)
        public void SendTestingResult_False_Notificeert_Attached_Observer()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var dev = CreateDeveloper(); // Developer is ook een observer
            item.AssignedDeveloper = dev; // Wijs toe (wordt automatisch genotificeerd)

            var mockObserver = new Mock<IObserver>(); // Maak een mock observer
            item.Attach(mockObserver.Object); // Voeg mock observer toe

            // Simuleer flow naar Testing state
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();

            // Act
            item.SendTestingResult(false); // Test faalt, zou moeten notificeren

            // Assert
            // Controleer of de Update methode van de mock observer is aangeroepen
            // met het juiste subject (item) en een bericht dat "FAILED" bevat.
            mockObserver.Verify(o => o.Update(item, It.Is<string>(s => s.Contains("FAILED"))), Times.Once);

            // Controleer ook of de toegewezen developer (die impliciet observer is) genotificeerd is.
            // Dit vereist mogelijk een aanpassing in User of een Mock<Developer> die Verify kan gebruiken.
            // Voor nu focussen we op de expliciete mock observer.
        }

        [Fact]
        // Requirement: FR06 (Observer pattern test)
        public void Detach_Observer_Voorkomt_Notificatie()
        {
            // Arrange
            var item = CreateNewBacklogItem();
            var mockObserver = new Mock<IObserver>();
            item.Attach(mockObserver.Object);
            item.Detach(mockObserver.Object); // Verwijder observer weer

            // Simuleer flow naar Testing state
            item.AssignedDeveloper = CreateDeveloper();
            item.StartTask();
            item.MarkAsReadyForTesting();
            item.StartTesting();

            // Act
            item.SendTestingResult(false); // Test faalt

            // Assert
            // Controleer dat de Update methode NIET is aangeroepen
            mockObserver.Verify(o => o.Update(It.IsAny<ISubject>(), It.IsAny<string>()), Times.Never);
        }
    }
}
