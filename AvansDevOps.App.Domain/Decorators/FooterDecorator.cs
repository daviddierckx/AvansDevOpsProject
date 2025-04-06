// AvansDevOps.App/Domain/Decorators/FooterDecorator.cs
using AvansDevOps.App.Domain.Interfaces.Patterns;
using System.Text;

namespace AvansDevOps.App.Domain.Decorators
{
    // Decorator Pattern: Concrete Decorator
    public class FooterDecorator : ReportDecorator
    {
        private string _confidentialityNotice;

        public FooterDecorator(IReportComponent component, string confidentialityNotice = "Confidential - For Internal Use Only")
            : base(component)
        {
            _confidentialityNotice = confidentialityNotice;
        }

        public override string GenerateContent()
        {
            var footer = new StringBuilder();
            footer.AppendLine(); // Lege regel
            footer.AppendLine("--------------------------------------------------");
            footer.AppendLine($" {_confidentialityNotice}");
            footer.AppendLine("--------------------------------------------------");


            // Voeg footer toe *na* de content van het wrapped component
            return base.GenerateContent() + footer.ToString();
        }
    }
}