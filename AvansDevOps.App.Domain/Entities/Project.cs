using AvansDevOps.App.Domain.Entities;
using AvansDevOps.App.Domain.Interfaces.Patterns;
using System.Collections.Generic;

namespace AvansDevOps.App.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductOwner ProductOwner { get; set; }
        public ProductBacklog ProductBacklog { get; private set; }
        public List<Sprint> Sprints { get; private set; }
        // Potentieel ook lijst van alle teamleden op projectniveau

        public Project(string name, ProductOwner productOwner)
        {
            Name = name;
            ProductOwner = productOwner;
            ProductBacklog = new ProductBacklog();
            Sprints = new List<Sprint>();
        }

        public void AddSprint(Sprint sprint)
        {
            Sprints.Add(sprint);
            // Eventueel logica om te zorgen dat sprint data niet overlapt etc.
        }

        public void AddBacklogItem(BacklogItem item)
        {
            ProductBacklog.AddItem(item);
        }
    }
}