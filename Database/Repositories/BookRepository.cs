using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace Database.Repositories;

public class BookRepository(DataContext context) : IBookRepository
{
	public bool Exists(Guid id)
	{
		return context.Books.Any(b => b.Id == id);
	}
	public Book Get(Guid id)
	{
		if (!Exists(id))
		{
			throw new ArgumentException("Book not found");
		}
		return context.Books.FirstOrDefault(b => b.Id == id);
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
		try
		{
			foreach (var id in authorIds)
			{
				var author = context.Authors.Where(a => a.Id == id).FirstOrDefault();
				if (author == null)
				{
					throw new NullReferenceException("Author not found");
				}
				AddBookAuthor(book, author);
			}

			context.Add(book);
			return Save();
		}
		catch
		{

			throw;
		}
	}
	public bool Update(Book book)
	{
		try
		{
			context.Update(book);
			return Save();
		}
		catch
		{
			throw;
		}
	}
	public bool Save()
	{
		try
		{
			return context.SaveChanges() > 0
				? true
				: throw new InvalidOperationException("Nothing to save");
		}
		catch
		{
			throw;
		}
	}
	public bool AddBookAuthor(Guid bookId, Guid authorId)
	{
		try
		{

			var author = context.Authors.Where(a => a.Id == authorId).FirstOrDefault();
			if (author == null)
			{
				throw new NullReferenceException("Author not found");
			}
			var book = context.Books.Where(b => b.Id == bookId).FirstOrDefault();
			if (book == null)
			{
				throw new NullReferenceException("Book not found");
			}
			return AddBookAuthor(book, author);
		}
		catch
		{
			throw;
		}
	}
	public bool RemoveBookAuthor(Guid bookId, Guid authorId)
	{
		try
		{

			var bookAuthor = context
				.BookAuthors
				.Where(ba => ba.AuthorId == authorId && ba.BookId == bookId)
				.FirstOrDefault();
			if (bookAuthor is null)
			{
				throw new Exception
					($"author id:{authorId}\" is not related with book id:{bookId}");
			}
			context.BookAuthors.Remove(bookAuthor);
			return Save();
		}
		catch
		{
			throw;
		}
	}
	public bool Delete(Guid id)
	{
		if (!Exists(id))
		{
			throw new ArgumentException($"Book ID: \"{id}\" does not exist");
		}
		try
		{
			context.BookAuthors.Where(ba => ba.BookId == id).ExecuteDelete();
			var book = Get(id);
			context.Remove(book);
			return Save();
		}
		catch
		{
			throw;
		}
	}
	private bool AddBookAuthor(Book book, Author author)
	{
		var bookAuthor = new BookAuthor()
		{
			Book = book,
			Author = author,
		};
		if (BookAuthorExists(book.Id, author.Id))
		{
			throw new InvalidOperationException
				($"author \"{author.Name}\" is already related with book \"{book.Title}\"");
		}
		context.Add(bookAuthor);
		return Save();
	}
	private bool BookAuthorExists(Guid bookId, Guid authorId)
	{
		return context.BookAuthors.Any(ba => ba.AuthorId == authorId && ba.BookId == bookId);
	}
}
