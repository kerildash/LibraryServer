using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IAuthorRepository : IRepository<Author>
{
	//Task<ICollection<Author>> GetAllAsync();
	public Task<ICollection<Author>> GetByBookId(Guid bookId);
	public Task<bool> Create(Author author);
}
