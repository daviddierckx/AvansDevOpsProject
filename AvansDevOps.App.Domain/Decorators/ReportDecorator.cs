// AvansDevOps.App/Domain/Decorators/ReportDecorator.cs
using AvansDevOps.App.Domain.Interfaces.Patterns;

namespace AvansDevOps.App.Domain.Decorators
{
    // Decorator Pattern: Abstract Decorator
    public abstract class ReportDecorator : IReportComponent
    {
        protected IReportComponent _wrappedComponent; // Het object dat gedecoreerd wordt

        protected ReportDecorator(IReportComponent component)
        {
            _wrappedComponent = component;
        }

        // Delegeer de basis operatie naar het wrapped component
        public virtual string GenerateContent()
        {
            return _wrappedComponent.GenerateContent();
        }
    }
}