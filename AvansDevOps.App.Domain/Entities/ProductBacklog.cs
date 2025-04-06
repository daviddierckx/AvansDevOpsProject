using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Domain.Entities
{
    public class ProductBacklog
    {
        public List<BacklogItem> Items { get; private set; }

        public ProductBacklog()
        {
            Items = new List<BacklogItem>();
        }

        public void AddItem(BacklogItem item)
        {
            Items.Add(item);
            // Potentieel sorteren op prioriteit
        }

        public void RemoveItem(BacklogItem item)
        {
            Items.Remove(item);
        }

        public List<BacklogItem> GetOrderedItems()
        {
            // Voor nu, simpele volgorde van toevoeging. Kan later complexer.
            return Items.ToList();
        }
    }
}