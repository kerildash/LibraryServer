using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IAuthorRepository : IRepository<Author>, IUpdatable<Author>
{
	//Task<ICollection<Author>> GetAllAsync();
	public Task<ICollection<Author>> GetByBookIdAsync(Guid bookId);
	public Task CreateAsync(Author author);
}
