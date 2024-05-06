
using Domain.Models;

namespace Database.RepositoryInterfaces
{
	public interface IStaticFileRepository : IRepository<StaticFile>
	{
		Task<ICollection<StaticFile>> GetByHolderId(Guid parentId);
		Task<bool> Create(StaticFile document);
	}
}
