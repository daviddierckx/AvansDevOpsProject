using System.Collections.Generic;
using System.Linq;

namespace AvansDevOps.App.Domain.Entities
{
    public class SprintBacklog
    {
        public List<BacklogItem> Items { get; private set; }

        public SprintBacklog()
        {
            Items = new List<BacklogItem>();
        }

        public void AddItem(BacklogItem item)
        {
            // Potentieel checken of item al in een andere sprint zit
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
        }

        public void RemoveItem(BacklogItem item)
        {
            Items.Remove(item);
            // Eventueel item terugplaatsen naar Product Backlog?
        }

        public bool AllItemsDone()
        {
            return Items.Any() && Items.All(item => item.IsDone());
        }
    }
}