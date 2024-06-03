namespace Domain.Models;

public class Author
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public required string Bio { get; set; }

	//references to actual files
	public Picture? Photo { get; set; }

	//join tables for many-to-many
	public ICollection<BookAuthor> BookAuthors { get; set; }
}
