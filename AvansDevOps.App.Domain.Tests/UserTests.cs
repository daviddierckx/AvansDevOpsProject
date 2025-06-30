using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using Moq;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class UserTests
    {
        private Developer CreateDeveloper(string name = "Test Dev")
        {
            return new Developer(name, name + "@test.com", name + "_slack");
        }

        [Fact]
        public void Test_AC_FR02_1_AddNotificationPreference_Adds_Strategy()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();
            Assert.Empty(user.NotificationPreferences);

            // Act
            user.AddNotificationPreference(mockStrategy.Object);

            // Assert
            Assert.Contains(mockStrategy.Object, user.NotificationPreferences);
            Assert.Single(user.NotificationPreferences);
        }

        [Fact]
        public void Test_FR02_AddNotificationPreference_Does_Not_Add_Duplicates()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();

            // Act
            user.AddNotificationPreference(mockStrategy.Object);
            user.AddNotificationPreference(mockStrategy.Object);

            // Assert
            Assert.Single(user.NotificationPreferences);
        }

        [Fact]
        public void Test_AC_FR02_2_RemoveNotificationPreference_Removes_Strategy()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy = new Mock<INotificationStrategy>();
            user.AddNotificationPreference(mockStrategy.Object);
            Assert.Single(user.NotificationPreferences);

            // Act
            user.RemoveNotificationPreference(mockStrategy.Object);

            // Assert
            Assert.Empty(user.NotificationPreferences);
        }

        [Fact]
        public void Test_FR02_RemoveNotificationPreference_Does_Nothing_If_Strategy_Not_Exists()
        {
            // Arrange
            var user = CreateDeveloper();
            var mockStrategy1 = new Mock<INotificationStrategy>();
            var mockStrategy2 = new Mock<INotificationStrategy>();
            user.AddNotificationPreference(mockStrategy1.Object);

            // Act
            user.RemoveNotificationPreference(mockStrategy2.Object);

            // Assert
            Assert.Single(user.NotificationPreferences);
            Assert.Contains(mockStrategy1.Object, user.NotificationPreferences);
        }
    }
}