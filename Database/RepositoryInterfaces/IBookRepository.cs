using Domain.Models;

namespace Database.RepositoryInterfaces;

public interface IBookRepository : IRepository<Book>
{
	ICollection<Book> GetByAuthorId(Guid authorId);
	ICollection<Book> GetByTagId(Guid tagId);
	bool Create(List<Guid> authorIds, Book book);
	bool AddBookAuthor(Guid bookId, Guid authorId);
	bool RemoveBookAuthor(Guid bookId, Guid authorId);
}
