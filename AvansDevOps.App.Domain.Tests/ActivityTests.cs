using AvansDevOps.App.Domain.Entities;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class ActivityTests
    {
        [Fact]
        // Requirement: FR03 (Activiteiten kunnen worden voltooid)
        public void MarkAsDone_Zet_Completed_Op_True()
        {
            // Arrange
            var activity = new Activity("Test deze activiteit");
            Assert.False(activity.Completed); // Pre-check

            // Act
            activity.MarkAsDone();

            // Assert
            Assert.True(activity.Completed);
            Assert.True(activity.IsDone()); // Controleer ook IsDone()
        }

        [Fact]
        // Requirement: FR03 (Nieuwe activiteit is niet voltooid)
        public void Nieuwe_Activity_Is_Niet_Completed()
        {
            // Arrange
            var activity = new Activity("Nieuwe activiteit");

            // Assert
            Assert.False(activity.Completed);
            Assert.False(activity.IsDone());
        }

        [Fact]
        // Requirement: FR03 (IsDone werkt correct)
        public void IsDone_Retourneert_Correcte_Waarde()
        {
            // Arrange
            var activity = new Activity("Test IsDone");

            // Assert initieel
            Assert.False(activity.IsDone());

            // Act
            activity.MarkAsDone();

            // Assert na MarkAsDone
            Assert.True(activity.IsDone());
        }

        [Fact]
        // Requirement: FR03 (MarkAsDone is idempotent)
        public void MarkAsDone_Meerdere_Keren_Aanroepen_Blijft_Completed_True()
        {
            // Arrange
            var activity = new Activity("Test Idempotentie");

            // Act
            activity.MarkAsDone(); // Eerste keer
            activity.MarkAsDone(); // Tweede keer

            // Assert
            Assert.True(activity.Completed);
            Assert.True(activity.IsDone());
        }
    }
}
