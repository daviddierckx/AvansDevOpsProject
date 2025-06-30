using AvansDevOps.App.Domain.Entities;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class ActivityTests
    {
        [Fact]
        public void Test_AC_FR05_20_MarkAsDone_Sets_Completed_To_True()
        {
            // Arrange
            var activity = new Activity("Test deze activiteit");
            Assert.False(activity.Completed);

            // Act
            activity.MarkAsDone();

            // Assert
            Assert.True(activity.Completed);
            Assert.True(activity.IsDone());
        }

        [Fact]
        public void Test_FR03_New_Activity_Is_Not_Completed()
        {
            // Arrange
            var activity = new Activity("Nieuwe activiteit");

            // Assert
            Assert.False(activity.Completed);
            Assert.False(activity.IsDone());
        }

        [Fact]
        public void Test_FR03_IsDone_Returns_Correct_Value()
        {
            // Arrange
            var activity = new Activity("Test IsDone");

            // Assert initial
            Assert.False(activity.IsDone());

            // Act
            activity.MarkAsDone();

            // Assert after
            Assert.True(activity.IsDone());
        }

        [Fact]
        public void Test_FR03_MarkAsDone_Is_Idempotent()
        {
            // Arrange
            var activity = new Activity("Test Idempotentie");

            // Act
            activity.MarkAsDone();
            activity.MarkAsDone();

            // Assert
            Assert.True(activity.Completed);
        }
    }
}