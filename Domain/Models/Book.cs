namespace Domain.Models;

public class Book
{
	public Guid Id { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	
	//references to actual files
	public Picture? Cover { get; set; }
	public required Document Document { get; set; }

	//join tables for many-to-many
	public ICollection<BookAuthor> BookAuthors { get; set; }
	public ICollection<BookTag> BookTags { get; set; }
}
