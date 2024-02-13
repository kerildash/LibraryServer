using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IAuthorRepository : IRepository<Author>
{
	ICollection<Author> GetByBookId(Guid bookId);
	public bool Create(Author author);
}
