using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface ITagRepository : IRepository<Tag>
{
	public Task<ICollection<Tag>> GetByBookId(Guid bookId);
	public Task<bool> Create(Tag tag);
}
