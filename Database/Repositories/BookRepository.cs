using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class BookRepository(DataContext context) : IBookRepository
{
	public async Task<bool> Exists(Guid id)
	{
		return await context.Books.AnyAsync(b => b.Id == id);
	}
	public async Task<Book> Get(Guid id)
	{
		if (!await Exists(id))
		{
			throw new ArgumentException("Book not found");
		}
		return await context.Books.FirstOrDefaultAsync(b => b.Id == id) ?? throw new NullReferenceException();
	}
	public async Task<ICollection<Book>> GetAll()
	{
		return await context.Books.ToListAsync();
	}
	public async Task<ICollection<Book>> Get(string title)
	{
		return await context.Books
			.Where(b => b.Title.Contains(title))
			.ToListAsync();
	}
	public async Task<ICollection<Book>> GetByAuthorId(Guid authorId)
	{
		return await context.BookAuthors
			.Where(ba => ba.AuthorId == authorId)
			.Select(ba => ba.Book)
			.ToListAsync();
	}
	public async Task<ICollection<Book>> GetByTagId(Guid tagId)
	{
		throw new NotImplementedException();
		//return context.BookTags
		//	.Where(ba => ba.TagId == tagId)
		//	.Select(ba => ba.Book)
		//	.ToList();
	}

	public async Task<bool> Create(List<Guid?> authorIds, Book book)
	{
		try
		{
			foreach (Guid? id in authorIds)
			{
				var author = await context.Authors.Where(a => a.Id == id).FirstOrDefaultAsync();
				if (author == null)
				{
					throw new NullReferenceException("Author not found");
				}
				await AddBookAuthor(book, author);
			}
			//Picture? cover = await context.Pictures.Where(p => p.Id == book.Cover.Id).FirstOrDefaultAsync();
			//cover.HolderId = book.Id;
			//context.Update(cover);

			//Document? document = await context.Documents.Where(d => d.Id == book.Document.Id).FirstOrDefaultAsync();
			//document.HolderId = book.Id;
			//context.Update(document);

			await context.AddAsync(book);
			return await Save();
		}
		catch
		{

			throw;
		}
	}
	public async Task<bool> Update(Book book)
	{
		try
		{
			context.Update(book);
			return await Save();
		}
		catch
		{
			throw;
		}
	}
	public async Task<bool> Save()
	{
		try
		{
			return await context.SaveChangesAsync() > 0
				? true
				: throw new InvalidOperationException("Nothing to save");
		}
		catch
		{
			throw;
		}
	}
	public async Task<bool> AddBookAuthor(Guid bookId, Guid authorId)
	{
		try
		{

			var author = await context.Authors.Where(a => a.Id == authorId).FirstOrDefaultAsync();
			if (author == null)
			{
				throw new NullReferenceException("Author not found");
			}
			var book = await context.Books.Where(b => b.Id == bookId).FirstOrDefaultAsync();
			if (book == null)
			{
				throw new NullReferenceException("Book not found");
			}
			return await AddBookAuthor(book, author);
		}
		catch
		{
			throw;
		}
	}
	public async Task<bool> RemoveBookAuthor(Guid bookId, Guid authorId)
	{
		try
		{

			var bookAuthor = await context
				.BookAuthors
				.Where(ba => ba.AuthorId == authorId && ba.BookId == bookId)
				.FirstOrDefaultAsync();
			if (bookAuthor is null)
			{
				throw new Exception
					($"author id:{authorId}\" is not related with book id:{bookId}");
			}
			context.BookAuthors.Remove(bookAuthor);
			return await Save();
		}
		catch
		{
			throw;
		}
	}
	public async Task<bool> Delete(Guid id)
	{
		if (!await Exists(id))
		{
			throw new ArgumentException($"Book ID: \"{id}\" does not exist");
		}
		try
		{
			await context.BookAuthors.Where(ba => ba.BookId == id).ExecuteDeleteAsync();
			var book = await Get(id);
			context.Remove(book);
			return await Save();
		}
		catch
		{
			throw;
		}
	}
	private async Task<bool> AddBookAuthor(Book book, Author author)
	{
		var bookAuthor = new BookAuthor()
		{
			Book = book,
			Author = author,
		};
		if (await BookAuthorExists(book.Id, author.Id))
		{
			throw new InvalidOperationException
				($"author \"{author.Name}\" is already related with book \"{book.Title}\"");
		}
		await context.AddAsync(bookAuthor);
		return await Save();
	}
	private async Task<bool> BookAuthorExists(Guid bookId, Guid authorId)
	{
		return await context.BookAuthors.AnyAsync(ba => ba.AuthorId == authorId && ba.BookId == bookId);
	}
}
