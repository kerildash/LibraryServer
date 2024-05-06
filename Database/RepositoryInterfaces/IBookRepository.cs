using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IBookRepository : IRepository<Book>
{
	Task<ICollection<Book>> GetByAuthorId(Guid authorId);
	Task<ICollection<Book>> GetByTagId(Guid tagId);
	Task<bool> Create(List<Guid?> authorIds, Book book);
	Task<bool> AddBookAuthor(Guid bookId, Guid authorId);
	Task<bool> RemoveBookAuthor(Guid bookId, Guid authorId);
}
