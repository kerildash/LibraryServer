namespace Domain.Models;

/// <summary>
/// Describes an actual file located in a folder 
/// </summary>
public class StaticFile
{
	public Guid Id { get; set; }
	public required string Path { get; set; }
	public Guid? HolderId { get; set; }

	public string GetName()
	{
		return System.IO.Path.GetFileName(Path);
	}
	public string GetExtension()
	{
		return System.IO.Path.GetExtension(Path);
	}
}
