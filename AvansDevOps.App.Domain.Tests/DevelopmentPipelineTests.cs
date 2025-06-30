using AvansDevOps.App.Domain.Entities;
using Moq;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class DevelopmentPipelineTests
    {
        [Fact]
        public void Test_FR07_Execute_Runs_All_Actions_On_Success()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Test Pipeline");
            var mockAction1 = new Mock<PipelineAction>("cmd1");
            var mockAction2 = new Mock<PipelineAction>("cmd2");

            mockAction1.Setup(a => a.Execute()).Returns(true);
            mockAction2.Setup(a => a.Execute()).Returns(true);

            pipeline.AddAction(mockAction1.Object);
            pipeline.AddAction(mockAction2.Object);

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.True(result);
            mockAction1.Verify(a => a.Execute(), Times.Once);
            mockAction2.Verify(a => a.Execute(), Times.Once);
        }

        [Fact]
        public void Test_FR07_Execute_Stops_On_Failing_Action()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Test Pipeline");
            var mockAction1 = new Mock<PipelineAction>("cmd1");
            var mockAction2 = new Mock<PipelineAction>("cmd2");

            mockAction1.Setup(a => a.Execute()).Returns(false);
            mockAction2.Setup(a => a.Execute()).Returns(true);

            pipeline.AddAction(mockAction1.Object);
            pipeline.AddAction(mockAction2.Object);

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.False(result);
            mockAction1.Verify(a => a.Execute(), Times.Once);
            mockAction2.Verify(a => a.Execute(), Times.Never);
        }

        [Fact]
        public void Test_FR07_Execute_Returns_True_For_Empty_Pipeline()
        {
            // Arrange
            var pipeline = new DevelopmentPipeline("Lege Pipeline");

            // Act
            bool result = pipeline.Execute();

            // Assert
            Assert.True(result);
        }
    }
}