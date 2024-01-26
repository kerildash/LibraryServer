using System.Net;

namespace Domain.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
     //   public ICollection<BookTag> BookTags { get; set; }
    }
}
