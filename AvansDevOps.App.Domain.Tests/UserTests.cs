
using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using Moq; // Voor mock strategie
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class UserTests
    {
        // Hulpfuncties
        private Developer CreateDeveloper(string name = "Test Dev")
        {
            return new Developer(name, name + "@test.com", name + "_slack");
        }

        [Fact]
        // Test voor AC-FR02.1: Given een User object (bv. Developer) en een INotificationStrategy (bv. Email) / When AddNotificationPreference wordt aangeroepen met de strategie / Then de NotificationPreferences lijst van de User bevat deze strategie.
        public void Test_AC_FR02_1_AddNotificationPreference_Voegt_Strategie_Toe()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();
            Assert.Empty(user.NotificationPreferences); // Pre-conditie

            // Act
            user.AddNotificationPreference(mockStrategy.Object);

            // Assert
            Assert.Contains(mockStrategy.Object, user.NotificationPreferences);
            Assert.Single(user.NotificationPreferences);
        }

        [Fact]
        // Gerelateerd aan FR02: AddNotificationPreference voegt dezelfde strategie niet dubbel toe.
        public void AddNotificationPreference_Voegt_Niet_Dubbel_Toe()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();

            // Act
            user.AddNotificationPreference(mockStrategy.Object); // Eerste keer
            user.AddNotificationPreference(mockStrategy.Object); // Tweede keer

            // Assert
            Assert.Single(user.NotificationPreferences); // Mag er maar één keer in staan
        }

        [Fact]
        // Test voor AC-FR02.2: Given een User object met een INotificationStrategy in de voorkeuren / When RemoveNotificationPreference wordt aangeroepen met die strategie / Then de NotificationPreferences lijst van de User bevat deze strategie niet meer.
        public void Test_AC_FR02_2_RemoveNotificationPreference_Verwijdert_Strategie()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();
            user.AddNotificationPreference(mockStrategy.Object);
            Assert.Single(user.NotificationPreferences); // Pre-conditie

            // Act
            user.RemoveNotificationPreference(mockStrategy.Object);

            // Assert
            Assert.Empty(user.NotificationPreferences);
        }

        [Fact]
        // Gerelateerd aan FR02: RemoveNotificationPreference doet niets als strategie niet bestaat.
        public void RemoveNotificationPreference_Doet_Niets_Als_Strategie_Niet_Bestaat()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy1 = new Mock<INotificationStrategy>();
            var mockStrategy2 = new Mock<INotificationStrategy>();
            user.AddNotificationPreference(mockStrategy1.Object); // Voeg alleen strategie 1 toe

            // Act
            user.RemoveNotificationPreference(mockStrategy2.Object); // Probeer strategie 2 te verwijderen

            // Assert
            Assert.Single(user.NotificationPreferences); // Lijst moet ongewijzigd zijn
            Assert.Contains(mockStrategy1.Object, user.NotificationPreferences);
        }

        // AC-FR02.3 wordt indirect getest via de notificatietests in SprintTests en BacklogItemTests
        // waar geverifieerd wordt dat observers (Users) hun Update methode aangeroepen krijgen.
        // Een directe test zou het mocken van INotificationService vereisen en controleren
        // of de juiste strategieën worden aangeroepen binnen die service, wat buiten de
        // scope van domein unit tests valt (meer Application/Infrastructure laag).
    }
}
