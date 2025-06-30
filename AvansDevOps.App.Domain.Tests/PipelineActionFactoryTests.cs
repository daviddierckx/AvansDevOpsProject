using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Factories;
using System;
using System.Collections.Generic;
using Xunit;

namespace AvansDevOps.App.Domain.Tests
{
    public class PipelineActionFactoryTests
    {
        [Fact]
        public void Test_FR07_CreateAction_SourceAction()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "RepositoryUrl", "git://test.com/repo.git" },
                { "Branch", "main" }
            };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Source, "Get Source", parameters);

            // Assert
            Assert.IsType<SourceAction>(action);
            var sourceAction = action as SourceAction;
            Assert.NotNull(sourceAction);
            Assert.Equal("git://test.com/repo.git", sourceAction.RepositoryUrl);
            Assert.Equal("main", sourceAction.Branch);
        }

        [Fact]
        public void Test_FR07_CreateAction_PackageAction()
        {
            // Arrange
            var packages = new List<string> { "PackageA", "PackageB" };
            var parameters = new Dictionary<string, object> { { "Packages", packages } };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Package, "Install Deps", parameters);

            // Assert
            Assert.IsType<PackageAction>(action);
            var packageAction = action as PackageAction;
            Assert.NotNull(packageAction);
            Assert.Equal(packages, packageAction.Packages);
        }

        [Fact]
        public void Test_FR07_CreateAction_BuildAction_With_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>();

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Build, "Build Project", parameters);

            // Assert
            Assert.IsType<BuildAction>(action);
            var buildAction = action as BuildAction;
            Assert.NotNull(buildAction);
            Assert.Equal("Release", buildAction.BuildConfiguration);
            Assert.Equal("Any CPU", buildAction.Platform);
        }

        [Fact]
        public void Test_FR07_CreateAction_BuildAction_With_Parameters()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
             {
                 { "Configuration", "Debug" },
                 { "Platform", "x64" }
             };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Build, "Build Debug x64", parameters);

            // Assert
            Assert.IsType<BuildAction>(action);
            var buildAction = action as BuildAction;
            Assert.NotNull(buildAction);
            Assert.Equal("Debug", buildAction.BuildConfiguration);
            Assert.Equal("x64", buildAction.Platform);
        }


        [Fact]
        public void Test_FR07_CreateAction_TestAction_With_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>();

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Test, "Run Tests", parameters);

            // Assert
            Assert.IsType<TestAction>(action);
            var testAction = action as TestAction;
            Assert.NotNull(testAction);
            Assert.Equal("NUnit", testAction.TestFramework);
            Assert.True(testAction.PublishResults);
            Assert.True(testAction.CollectCoverage);
        }

        [Fact]
        public void Test_FR07_CreateAction_TestAction_With_Parameters()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "TestFramework", "xUnit" },
                { "PublishResults", false },
                { "CollectCoverage", false },
                { "ShouldFail", true }
            };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Test, "Run Failing xUnit Tests", parameters);

            // Assert
            Assert.IsType<TestAction>(action);
            var testAction = action as TestAction;
            Assert.NotNull(testAction);
            Assert.Equal("xUnit", testAction.TestFramework);
            Assert.False(testAction.PublishResults);
            Assert.False(testAction.CollectCoverage);
        }

        [Fact]
        public void Test_FR07_CreateAction_AnalyseAction_With_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>();

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Analyse, "Static Analysis", parameters);

            // Assert
            Assert.IsType<AnalyseAction>(action);
            var analyseAction = action as AnalyseAction;
            Assert.NotNull(analyseAction);
            Assert.Equal("SonarQube", analyseAction.Tool);
            Assert.Null(analyseAction.SettingsFile);
        }

        [Fact]
        public void Test_FR07_CreateAction_AnalyseAction_With_Parameters()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "Tool", "CustomLinter" },
                { "SettingsFile", "lint.config" }
            };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Analyse, "Custom Linting", parameters);

            // Assert
            Assert.IsType<AnalyseAction>(action);
            var analyseAction = action as AnalyseAction;
            Assert.NotNull(analyseAction);
            Assert.Equal("CustomLinter", analyseAction.Tool);
            Assert.Equal("lint.config", analyseAction.SettingsFile);
        }


        [Fact]
        public void Test_FR07_CreateAction_DeployAction()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "Environment", "Production" },
                { "ServerAddress", "prod.server.com" }
            };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Deploy, "Deploy Prod", parameters);

            // Assert
            Assert.IsType<DeployAction>(action);
            var deployAction = action as DeployAction;
            Assert.NotNull(deployAction);
            Assert.Equal("Production", deployAction.Environment);
            Assert.Equal("prod.server.com", deployAction.ServerAddress);
        }

        [Fact]
        public void Test_FR07_CreateAction_UtilityAction()
        {
            // Arrange
            var args = new List<string> { "/source", "/dest" };
            var parameters = new Dictionary<string, object>
            {
                { "Command", "copy" },
                { "Arguments", args }
            };

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Utility, "Copy Files", parameters);

            // Assert
            Assert.IsType<UtilityAction>(action);
            var utilityAction = action as UtilityAction;
            Assert.NotNull(utilityAction);
            Assert.Equal("copy", utilityAction.Command);
            Assert.Equal(args, utilityAction.Arguments);
        }


        [Fact]
        public void Test_FR07_CreateAction_ThrowsException_On_Missing_Source_Parameter()
        {
            // Arrange
            var parameters = new Dictionary<string, object> { { "Branch", "main" } };
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Source, "Get Source", parameters));
            Assert.Contains("RepositoryUrl", ex.Message);
        }

        [Fact]
        public void Test_FR07_CreateAction_ThrowsException_On_Invalid_Package_Parameter()
        {
            // Arrange
            var parameters = new Dictionary<string, object> { { "Packages", "not-a-list" } };
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Package, "Install Deps", parameters));
            Assert.Contains("Packages (List<string>)", ex.Message);
        }

        [Fact]
        public void Test_FR07_CreateAction_ThrowsException_On_Missing_Deploy_Parameter()
        {
            // Arrange
            var parameters = new Dictionary<string, object> { { "Environment", "Production" } };
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Deploy, "Deploy Prod", parameters));
            Assert.Contains("ServerAddress", ex.Message);
        }

        [Fact]
        public void Test_FR07_CreateAction_ThrowsException_On_Missing_Utility_Parameter()
        {
            // Arrange
            var args = new List<string> { "/source", "/dest" };
            var parameters = new Dictionary<string, object> { { "Arguments", args } };
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Utility, "Copy Files", parameters));
            Assert.Contains("Command", ex.Message);
        }

        [Fact]
        public void Test_FR07_CreateAction_ThrowsException_On_Unknown_ActionType()
        {
            // Arrange
            var parameters = new Dictionary<string, object>();
            var invalidType = (PipelineActionFactory.ActionType)99;
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => PipelineActionFactory.CreateAction(invalidType, "Invalid Action", parameters));
        }
    }
}