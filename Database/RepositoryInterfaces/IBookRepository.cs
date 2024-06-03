using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IBookRepository : IRepository<Book>, IUpdatable<Book>
{
	Task<ICollection<Book>> GetByAuthorIdAsync(Guid authorId);
	Task<ICollection<Book>> GetByTagIdAsync(Guid tagId);
	Task CreateAsync(List<Guid?> authorIds, Book book);
	Task AddBookAuthorAsync(Guid bookId, Guid authorId);
	Task RemoveBookAuthorAsync(Guid bookId, Guid authorId);
}
