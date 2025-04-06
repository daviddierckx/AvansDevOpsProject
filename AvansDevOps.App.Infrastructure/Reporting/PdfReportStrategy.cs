using AvansDevOps.App.Domain.Interfaces.Patterns;
using AvansDevOps.App.Domain.Interfaces.Strategies;
using AvansDevOps.App.Domain.ValueObjects;
using System;
using System.IO; // Voor file path

namespace AvansDevOps.App.Infrastructure.Reporting
{
    // Strategy Pattern: Concrete Strategy
    public class PdfReportStrategy : IReportGenerationStrategy
    {
        public ReportFormat GetFormat()
        {
            return ReportFormat.Pdf;
        }

        public void GenerateReport(IReportComponent reportComponent, string filePath)
        {
            string content = reportComponent.GenerateContent(); // Haal content op (via Decorator)

            Console.WriteLine($"--- Generating PDF Report ---");
            Console.WriteLine($"   Target File: {filePath}.pdf"); // Voeg extensie toe

            // Simulatie van PDF generatie
            Console.WriteLine("   Initializing PDF library (simulation)...");
            Console.WriteLine("   Adding content to PDF document...");
            Console.WriteLine($"   Saving PDF document to {filePath}.pdf ...");

            // Fake saving to file
            try
            {
                // Voeg .pdf extensie toe als die mist
                string finalPath = filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? filePath : filePath + ".pdf";
                File.WriteAllText(finalPath, $"PDF REPORT\n==========\n\n{content}");
                Console.WriteLine("   PDF report generated successfully (simulation).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ERROR saving simulated PDF report: {ex.Message}");
            }
            Console.WriteLine($"--- End PDF Report Generation ---");
        }
    }
}