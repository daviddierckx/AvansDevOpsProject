namespace AvansDevOps.App.Domain.Entities
{
    public class TestAction : PipelineAction
    {
        public string TestFramework { get; private set; } // e.g., "NUnit", "xUnit"
        public bool PublishResults { get; private set; }
        public bool CollectCoverage { get; private set; }

        // Simulatie van testfalen
        private bool _shouldFail;

        public TestAction(string name, string testFramework = "NUnit", bool publish = true, bool coverage = true, bool shouldFail = false) : base(name)
        {
            TestFramework = testFramework;
            PublishResults = publish;
            CollectCoverage = coverage;
            _shouldFail = shouldFail;
        }

        public override bool Execute()
        {
            Console.WriteLine($"   Running tests using {TestFramework}...");
            if (CollectCoverage) Console.WriteLine("   Collecting code coverage...");

            // Simulatie
            bool testsPassed = !_shouldFail; // Laat de test slagen tenzij anders ingesteld

            if (testsPassed)
            {
                Console.WriteLine("   All tests passed.");
                if (PublishResults) Console.WriteLine("   Publishing test results...");
                return true;
            }
            else
            {
                Console.WriteLine("   !!! Some tests failed! !!!");
                if (PublishResults) Console.WriteLine("   Publishing test failure results...");
                return false; // Falen
            }
        }
    }
}