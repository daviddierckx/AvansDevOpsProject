using AvansDevOps.App.Domain.Interfaces.Patterns; // Voor IReportComponent
using AvansDevOps.App.Domain.ValueObjects; // Voor ReportFormat

namespace AvansDevOps.App.Domain.Interfaces.Strategies
{
    // Strategy Pattern Interface
    public interface IReportGenerationStrategy
    {
        void GenerateReport(IReportComponent reportComponent, string filePath);
        ReportFormat GetFormat(); // Om te weten welk format deze strategie genereert
    }
}