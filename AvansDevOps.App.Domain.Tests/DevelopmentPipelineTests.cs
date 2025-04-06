
using AvansDevOps.App.Domain.Entities;
using System; 
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class DevelopmentPipelineTests
    {
        // --- Concrete Test Implementatie van PipelineAction ---
        // Vervangt de Moq setup voor PipelineAction om NotSupportedException te voorkomen.
        private class TestPipelineAction : PipelineAction
        {
            private readonly bool _shouldSucceed;
            public int ExecuteCallCount { get; private set; } = 0;

            public TestPipelineAction(string name, bool shouldSucceed) : base(name)
            {
                _shouldSucceed = shouldSucceed;
            }

            // Implementeer de abstracte Execute methode
            public override bool Execute()
            {
                ExecuteCallCount++; // Houd bij hoe vaak deze is aangeroepen
                Console.WriteLine($"--- Executing Test Action: {Name} (Will Succeed: {_shouldSucceed}) ---"); // Optionele output
                return _shouldSucceed;
            }
        }
        // --- Einde Test Implementatie ---


        [Fact]
        // Requirement: FR07 (Pipeline voert acties uit)
        public void Execute_Voert_Alle_Acties_Uit_Bij_Succes()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Succes Pipeline");
            // Gebruik de concrete test implementatie ipv Moq
            var action1 = new TestPipelineAction("Action1", true);
            var action2 = new TestPipelineAction("Action2", true);

            pipeline.AddAction(action1);
            pipeline.AddAction(action2);

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.True(result); // Pipeline moet succesvol zijn
            Assert.Equal(1, action1.ExecuteCallCount); // Controleer of actie 1 is uitgevoerd
            Assert.Equal(1, action2.ExecuteCallCount); // Controleer of actie 2 is uitgevoerd
        }

        [Fact]
        // Requirement: FR07 (Pipeline stopt bij falen)
        // Path Coverage: Test het pad waarbij een actie faalt.
        public void Execute_Stopt_Na_Falende_Actie()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Fail Pipeline");
            // Gebruik de concrete test implementatie ipv Moq
            var action1 = new TestPipelineAction("Action1", true);
            var action2 = new TestPipelineAction("Action2 - Fails", false); // Deze faalt
            var action3 = new TestPipelineAction("Action3", true);

            pipeline.AddAction(action1);
            pipeline.AddAction(action2);
            pipeline.AddAction(action3);

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.False(result); // Pipeline moet falen
            Assert.Equal(1, action1.ExecuteCallCount); // Actie 1 uitgevoerd
            Assert.Equal(1, action2.ExecuteCallCount); // Actie 2 uitgevoerd (en faalde)
            Assert.Equal(0, action3.ExecuteCallCount); // Actie 3 mag NIET uitgevoerd zijn
        }

        [Fact]
        // Requirement: FR07 (Lege pipeline)
        // Path Coverage: Test het pad met een lege actielijst.
        public void Execute_Is_Succesvol_Voor_Lege_Pipeline()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Lege Pipeline");

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.True(result); // Aanname: lege pipeline is succesvol
        }
    }
}
