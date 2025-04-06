using AvansDevOps.App.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Factories
{
    // Factory Method Pattern (of Abstract Factory als je meerdere factory types had)
    // Deze factory maakt PipelineAction objecten aan.
    public static class PipelineActionFactory
    {
        // Gebruik enums voor duidelijkheid ipv strings
        public enum ActionType { Source, Package, Build, Test, Analyse, Deploy, Utility }

        // Factory Method
        public static PipelineAction CreateAction(ActionType type, string name, Dictionary<string, object> parameters)
        {
            switch (type)
            {
                case ActionType.Source:
                    // Validatie en veilige casting
                    if (!parameters.TryGetValue("RepositoryUrl", out var repoUrlObj) || !(repoUrlObj is string repoUrl))
                        throw new ArgumentException("Missing or invalid parameter: RepositoryUrl (string)");
                    if (!parameters.TryGetValue("Branch", out var branchObj) || !(branchObj is string branch))
                        throw new ArgumentException("Missing or invalid parameter: Branch (string)");
                    return new SourceAction(name, repoUrl, branch);

                case ActionType.Package:
                    if (!parameters.TryGetValue("Packages", out var packagesObj) || !(packagesObj is List<string> packages))
                        throw new ArgumentException("Missing or invalid parameter: Packages (List<string>)");
                    return new PackageAction(name, packages);

                case ActionType.Build:
                    string config = parameters.GetValueOrDefault("Configuration", "Release") as string;
                    string platform = parameters.GetValueOrDefault("Platform", "Any CPU") as string;
                    return new BuildAction(name, config, platform);

                case ActionType.Test:
                    string framework = parameters.GetValueOrDefault("TestFramework", "NUnit") as string;
                    bool publish = (bool)parameters.GetValueOrDefault("PublishResults", true);
                    bool coverage = (bool)parameters.GetValueOrDefault("CollectCoverage", true);
                    bool shouldFail = (bool)parameters.GetValueOrDefault("ShouldFail", false); // Voor simulatie
                    return new TestAction(name, framework, publish, coverage, shouldFail);

                case ActionType.Analyse:
                    string tool = parameters.GetValueOrDefault("Tool", "SonarQube") as string;
                    string settings = parameters.GetValueOrDefault("SettingsFile", null) as string;
                    return new AnalyseAction(name, tool, settings);

                case ActionType.Deploy:
                    if (!parameters.TryGetValue("Environment", out var envObj) || !(envObj is string env))
                        throw new ArgumentException("Missing or invalid parameter: Environment (string)");
                    if (!parameters.TryGetValue("ServerAddress", out var serverObj) || !(serverObj is string server))
                        throw new ArgumentException("Missing or invalid parameter: ServerAddress (string)");
                    bool deployShouldFail = (bool)parameters.GetValueOrDefault("ShouldFail", false); // Voor simulatie
                    return new DeployAction(name, env, server, deployShouldFail);

                case ActionType.Utility:
                    if (!parameters.TryGetValue("Command", out var cmdObj) || !(cmdObj is string cmd))
                        throw new ArgumentException("Missing or invalid parameter: Command (string)");
                    if (!parameters.TryGetValue("Arguments", out var argsObj) || !(argsObj is List<string> args))
                        throw new ArgumentException("Missing or invalid parameter: Arguments (List<string>)");
                    return new UtilityAction(name, cmd, args);

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported action type: {type}");
            }
        }
    }

    // Extension method voor Dictionary voor leesbaarheid
    internal static class DictionaryExtensions
    {
        public static T GetValueOrDefault<T>(this Dictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.TryGetValue(key, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return defaultValue;
        }
    }
}