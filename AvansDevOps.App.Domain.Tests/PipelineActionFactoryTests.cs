
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
        // Requirement: FR07 (Factory Pattern - Source)
        public void CreateAction_Maakt_Correcte_SourceAction()
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
            Assert.NotNull(sourceAction); // Controleer cast
            Assert.Equal("git://test.com/repo.git", sourceAction.RepositoryUrl);
            Assert.Equal("main", sourceAction.Branch);
            Assert.Equal("Get Source", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Package)
        public void CreateAction_Maakt_Correcte_PackageAction()
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
            Assert.Equal("Install Deps", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Build)
        public void CreateAction_Maakt_Correcte_BuildAction_Met_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>(); // Gebruik defaults

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Build, "Build Project", parameters);

            // Assert
            Assert.IsType<BuildAction>(action);
            var buildAction = action as BuildAction;
            Assert.NotNull(buildAction);
            Assert.Equal("Release", buildAction.BuildConfiguration); // Default
            Assert.Equal("Any CPU", buildAction.Platform); // Default
            Assert.Equal("Build Project", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Build met parameters)
        public void CreateAction_Maakt_Correcte_BuildAction_Met_Parameters()
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
            Assert.Equal("Build Debug x64", action.Name);
        }


        [Fact]
        // Requirement: FR07 (Factory Pattern - Test)
        public void CreateAction_Maakt_Correcte_TestAction_Met_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>(); // Geen parameters, gebruik defaults

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Test, "Run Tests", parameters);

            // Assert
            Assert.IsType<TestAction>(action);
            var testAction = action as TestAction;
            Assert.NotNull(testAction); // Controleer cast
            Assert.Equal("NUnit", testAction.TestFramework); // Default
            Assert.True(testAction.PublishResults); // Default
            Assert.True(testAction.CollectCoverage); // Default
            Assert.Equal("Run Tests", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Test met parameters)
        public void CreateAction_Maakt_Correcte_TestAction_Met_Parameters()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "TestFramework", "xUnit" },
                { "PublishResults", false },
                { "CollectCoverage", false },
                { "ShouldFail", true } // Voor simulatie
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
            // Assert.True(testAction._shouldFail); // Kan niet direct private field testen
            Assert.Equal("Run Failing xUnit Tests", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Analyse)
        public void CreateAction_Maakt_Correcte_AnalyseAction_Met_Defaults()
        {
            // Arrange
            var parameters = new Dictionary<string, object>(); // Gebruik defaults

            // Act
            var action = PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Analyse, "Static Analysis", parameters);

            // Assert
            Assert.IsType<AnalyseAction>(action);
            var analyseAction = action as AnalyseAction;
            Assert.NotNull(analyseAction);
            Assert.Equal("SonarQube", analyseAction.Tool); // Default
            Assert.Null(analyseAction.SettingsFile); // Default
            Assert.Equal("Static Analysis", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Analyse met parameters)
        public void CreateAction_Maakt_Correcte_AnalyseAction_Met_Parameters()
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
            Assert.Equal("Custom Linting", action.Name);
        }


        [Fact]
        // Requirement: FR07 (Factory Pattern - Deploy)
        public void CreateAction_Maakt_Correcte_DeployAction()
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
            Assert.Equal("Deploy Prod", action.Name);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - Utility)
        public void CreateAction_Maakt_Correcte_UtilityAction()
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
            Assert.Equal("Copy Files", action.Name);
        }


        [Fact]
        // Requirement: FR07 (Factory Pattern - foutafhandeling Source)
        public void CreateAction_Gooit_Exception_Bij_Missende_Parameter_Voor_Source()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                // "RepositoryUrl" mist
                { "Branch", "main" }
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Source, "Get Source", parameters)
            );
            Assert.Contains("RepositoryUrl", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - foutafhandeling Package)
        public void CreateAction_Gooit_Exception_Bij_Ongeldig_Parameter_Type_Voor_Package()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "Packages", "geen list" } // Ongeldig type
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Package, "Install Deps", parameters)
            );
            Assert.Contains("Packages (List<string>)", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - foutafhandeling Deploy)
        public void CreateAction_Gooit_Exception_Bij_Missende_Parameter_Voor_Deploy()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "Environment", "Production" }
                // "ServerAddress" mist
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Deploy, "Deploy Prod", parameters)
            );
            Assert.Contains("ServerAddress", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - foutafhandeling Utility)
        public void CreateAction_Gooit_Exception_Bij_Missende_Parameter_Voor_Utility()
        {
            // Arrange
            var args = new List<string> { "/source", "/dest" };
            var parameters = new Dictionary<string, object>
            {
                // "Command" mist
                { "Arguments", args }
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                PipelineActionFactory.CreateAction(PipelineActionFactory.ActionType.Utility, "Copy Files", parameters)
            );
            Assert.Contains("Command", exception.Message);
        }

        [Fact]
        // Requirement: FR07 (Factory Pattern - onbekend type)
        public void CreateAction_Gooit_Exception_Bij_Onbekend_ActionType()
        {
            // Arrange
            var parameters = new Dictionary<string, object>();
            var invalidType = (PipelineActionFactory.ActionType)99; // Ongeldige enum waarde

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                PipelineActionFactory.CreateAction(invalidType, "Invalid Action", parameters)
            );
        }
    }
}
