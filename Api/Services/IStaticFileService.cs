using Domain.Models;

namespace Api.Services;

public interface IStaticFileService<T> where T: StaticFile, new()
{
	internal Task<T> CreateAsync(IFormFile staticFileCreate);
	internal bool IsValidExtension(IFormFile file);

}
