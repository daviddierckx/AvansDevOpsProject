using AvansDevOps.App.Domain.Entities;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class ProjectTests
    {
        private ProductOwner CreateProductOwner() => new ProductOwner("Test PO", "po@test.com");

        [Fact]
        public void Test_AC_FR01_1_CreateProject_With_Correct_Properties()
        {
            // Arrange
            var po = CreateProductOwner();
            var projectName = "Nieuw Testproject";

            // Act
            var project = new Project(projectName, po);

            // Assert
            Assert.Equal(projectName, project.Name);
            Assert.Equal(po, project.ProductOwner);
            Assert.NotNull(project.ProductBacklog);
        }

        [Fact]
        public void Test_AC_FR03_2_AddBacklogItem_To_ProjectBacklog()
        {
            // Arrange
            var project = new Project("Test Project", CreateProductOwner());
            var backlogItem = new BacklogItem("Nieuw Item", "Beschrijving");
            Assert.Empty(project.ProductBacklog.Items);

            // Act
            project.AddBacklogItem(backlogItem);

            // Assert
            Assert.Single(project.ProductBacklog.Items);
            Assert.Contains(backlogItem, project.ProductBacklog.Items);
        }
    }
}