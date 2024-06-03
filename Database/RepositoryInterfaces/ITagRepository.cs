using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface ITagRepository : IRepository<Tag>, IUpdatable<Tag>
{
	public Task<ICollection<Tag>> GetByBookIdAsync(Guid bookId);
	public Task CreateAsync(Tag tag);
}
