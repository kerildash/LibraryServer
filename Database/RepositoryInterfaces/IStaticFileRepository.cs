
using Domain.Models;

namespace Database.RepositoryInterfaces
{
	public interface IStaticFileRepository<T> : IRepository<T>, IUpdatable<T> where T : StaticFile
	{
		Task<ICollection<T>> GetByHolderIdAsync(Guid holderId);
		Task CreateAsync(T file);
	}
}
