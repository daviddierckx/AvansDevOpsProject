using System;

namespace AvansDevOps.App.Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public User Author { get; set; }
        public DateTime Timestamp { get; private set; }

        public Message(string content, User author)
        {
            Content = content;
            Author = author;
            Timestamp = DateTime.Now;
        }
    }
}