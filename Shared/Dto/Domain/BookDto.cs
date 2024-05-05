namespace Shared.Dto.Domain;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid PictureId { get; set; }
    public Guid DocumentId { get; set; }
}
