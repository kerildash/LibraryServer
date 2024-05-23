namespace Shared.Dto.Domain;

public class BookDto
{
    public string? Id { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid DocumentId { get; set; }

	public Guid CoverId { get; set; }
}
