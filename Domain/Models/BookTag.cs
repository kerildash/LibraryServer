namespace Domain.Models;

/// <summary>
/// A join table to model a <em>many-to many</em> relationship between <strong>Books</strong> and <strong>Tags</strong> tables
/// </summary>
public class BookTag
{
	public Guid BookId { get; set; }
	public Guid TagId { get; set; }
	public required Book Book { get; set; }
	public required Tag Tag { get; set; }
}