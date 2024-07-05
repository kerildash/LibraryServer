using Domain.Models;

namespace Api.Services.Interfaces;

public interface IStaticFileService<T> where T : StaticFile
{
    internal Task<T> CreateAsync(IFormFile staticFileCreate);
    internal bool IsValidExtension(IFormFile file);
}
