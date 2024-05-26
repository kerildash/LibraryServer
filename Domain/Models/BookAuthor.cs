namespace Domain.Models;

/// <summary>
/// A join table to model a <em>many-to many</em>
/// relationship between <strong>Books</strong> and <strong>Authors</strong> tables
/// </summary>
public class BookAuthor
{
	public Guid BookId { get; set; }
	public Guid AuthorId { get; set; }
	public required Book Book { get; set; }
	public required Author Author { get; set; }
}
