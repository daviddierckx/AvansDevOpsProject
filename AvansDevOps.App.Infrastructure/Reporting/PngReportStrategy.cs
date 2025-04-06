using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using AvansDevOps.App.Domain.ValueObjects;
using System;
using System.IO; // Voor file path

namespace AvansDevOps.App.Infrastructure.Reporting
{
    // Strategy Pattern: Concrete Strategy
    public class PngReportStrategy : IReportGenerationStrategy
    {
        public ReportFormat GetFormat()
        {
            return ReportFormat.Png;
        }

        public void GenerateReport(IReportComponent reportComponent, string filePath)
        {
            string content = reportComponent.GenerateContent();

            Console.WriteLine($"--- Generating PNG Report ---");
            Console.WriteLine($"   Target File: {filePath}.png"); // Voeg extensie toe

            // Simulatie van PNG generatie (bv. van een grafiek of de tekst renderen)
            Console.WriteLine("   Initializing Graphics library (simulation)...");
            Console.WriteLine("   Rendering report content to image buffer...");
            Console.WriteLine($"   Saving PNG image to {filePath}.png ...");

            // Fake saving to file
            try
            {
                string finalPath = filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? filePath : filePath + ".png";
                File.WriteAllText(finalPath, $"PNG IMAGE DATA PLACEHOLDER\n=========================\n\nContent Hash: {content.GetHashCode()}"); // Simuleer image data
                Console.WriteLine("   PNG report generated successfully (simulation).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ERROR saving simulated PNG report: {ex.Message}");
            }
            Console.WriteLine($"--- End PNG Report Generation ---");
        }
    }
}