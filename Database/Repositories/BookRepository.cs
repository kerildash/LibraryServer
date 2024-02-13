using Database.RepositoryInterfaces;
using Domain.Models;

namespace Database.Repositories;

public class BookRepository(DataContext context) : IBookRepository
{
	public bool Exists(Guid id)
	{
		return context.Books.Any(b => b.Id == id);
	}
	public Book? Get(Guid id)
	{
		return context.Books
			.Where(b => b.Id == id)
			.FirstOrDefault();
	}
	public ICollection<Book> GetAll()
	{
		return context.Books.ToList();
	}
	public ICollection<Book> Get(string title)
	{
		return context.Books
			.Where(b => b.Title.Contains(title))
			.ToList();
	}

	public ICollection<Book> GetByAuthorId(Guid authorId)
	{
		return context.BookAuthors
			.Where(ba => ba.AuthorId == authorId)
			.Select(ba => ba.Book)
			.ToList();
	}
	public ICollection<Book> GetByTagId(Guid tagId)
	{
		throw new NotImplementedException();
		//return context.BookTags
		//	.Where(ba => ba.TagId == tagId)
		//	.Select(ba => ba.Book)
		//	.ToList();
	}

	public bool Create(List<Guid> authorIds, Book book)
	{
		foreach (var id in authorIds)
		{
			var author = context.Authors.Where(a => a.Id == id).FirstOrDefault();
			var bookAuthor = new BookAuthor()
			{
				Book = book,
				Author = author,
			};
			context.Add(bookAuthor);
		}
		
		context.Add(book);
		return Save();
	}
	public bool Save()
	{
		return context.SaveChanges() > 0 ? true : false;
	}


}
