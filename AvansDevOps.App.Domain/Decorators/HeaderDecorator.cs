﻿// AvansDevOps.App/Domain/Decorators/HeaderDecorator.cs
using AvansDevOps.App.Domain.Interfaces.Patterns;
using System.Text;

namespace AvansDevOps.App.Domain.Decorators
{
    // Decorator Pattern: Concrete Decorator
    public class HeaderDecorator : ReportDecorator
    {
        private string _companyName;
        private string _logoPath; // Optioneel

        public HeaderDecorator(IReportComponent component, string companyName, string logoPath = null)
            : base(component)
        {
            _companyName = companyName;
            _logoPath = logoPath;
        }

        public override string GenerateContent()
        {
            var header = new StringBuilder();
            header.AppendLine("==================================================");
            header.AppendLine($" Report Generated by: {_companyName}");
            if (!string.IsNullOrEmpty(_logoPath))
            {
                header.AppendLine($" [Logo Placeholder: {_logoPath}]");
            }
            header.AppendLine($" Generation Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            header.AppendLine("==================================================");
            header.AppendLine(); // Lege regel

            // Voeg header toe *voor* de content van het wrapped component
            return header.ToString() + base.GenerateContent();
        }
    }
}