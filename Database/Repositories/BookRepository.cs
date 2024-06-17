using Database.RepositoryInterfaces;
using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class BookRepository(DataContext context, ISearchService<Book> search) : IBookRepository
{
	public async Task<bool> ExistsAsync(Guid id)
	{
		return await context.Books.AnyAsync(b => b.Id == id);
	}
	public async Task<Book> GetAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException("Book not found");
		}
		return await context.Books
			.Include(b => b.Cover)
			.Include(b => b.Document)
			.Include(b => b.BookAuthors)
			.ThenInclude(ba => ba.Author)
			.Include(b => b.BookTags)
			.ThenInclude(ba => ba.Tag)
			.FirstOrDefaultAsync(b => b.Id == id)
			?? throw new NullReferenceException();
	}
	public async Task<ICollection<Book>> GetAllAsync()
	{
		var books = await context.Books
			.Include(b => b.Cover)
			.Include(b => b.Document)
			.Include(b => b.BookAuthors)
			.ThenInclude(ba => ba.Author)
			.Include(b => b.BookTags)
			.ThenInclude(ba => ba.Tag)
			.ToListAsync();
		return books;
	}
	public async Task<ICollection<Book>> GetAsync(string query)
	{
		return await search.FindAsync(query);
	}
	public async Task<ICollection<Book>> GetByAuthorIdAsync(Guid authorId)
	{
		return await context.BookAuthors
			.Where(ba => ba.AuthorId == authorId)
			.Select(ba => ba.Book)
			.Include(b => b.Cover)
			.Include(b => b.Document)
			.Include(b => b.BookAuthors)
			.ThenInclude(ba => ba.Author)
			.Include(b => b.BookTags)
			.ThenInclude(ba => ba.Tag)
			.ToListAsync();
	}
	public async Task<ICollection<Book>> GetByTagIdAsync(Guid tagId)
	{
		return await context.BookTags
			.Where(bt => bt.TagId == tagId)
			.Select(bt => bt.Book)
			.Include(b => b.Cover)
			.Include(b => b.Document)
			.Include(b => b.BookAuthors)
			.ThenInclude(ba => ba.Author)
			.Include(b => b.BookTags)
			.ThenInclude(ba => ba.Tag)
			.ToListAsync();
	}

	public async Task CreateAsync(List<Guid?> authorIds, Book book)
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
				await AddBookAuthorAsync(book, author);
			}

			await context.AddAsync(book);
			await SaveAsync();
		}
		catch
		{
			throw;
		}
	}
	public async Task UpdateAsync(Book book)
	{
		try
		{
			context.Update(book);
			await SaveAsync();
		}
		catch
		{
			throw;
		}
	}
	public async Task DeleteAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException($"Book ID: \"{id}\" does not exist");
		}
		try
		{
			await context.BookAuthors.Where(ba => ba.BookId == id).ExecuteDeleteAsync();
			var book = await GetAsync(id);
			context.Remove(book);
			await SaveAsync();
		}
		catch
		{
			throw;
		}
	}
	public async Task SaveAsync()
	{
		await context.SaveChangesAsync();
	}
	public async Task AddBookAuthorAsync(Guid bookId, Guid authorId)
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
			await AddBookAuthorAsync(book, author);
		}
		catch
		{
			throw;
		}
	}
	public async Task RemoveBookAuthorAsync(Guid bookId, Guid authorId)
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
			await SaveAsync();
		}
		catch
		{
			throw;
		}
	}
	
	private async Task AddBookAuthorAsync(Book book, Author author)
	{
		if (await BookAuthorExistsAsync(book.Id, author.Id))
		{
			throw new InvalidOperationException
				($"author \"{author.Name}\" is already related with book \"{book.Title}\"");
		}
		var bookAuthor = new BookAuthor()
		{
			Book = book,
			Author = author,
			BookId = book.Id,
			AuthorId = author.Id
		};
		
		await context.AddAsync(bookAuthor);
		await SaveAsync();
	}
	private async Task<bool> BookAuthorExistsAsync(Guid bookId, Guid authorId)
	{
		return await context.BookAuthors.AnyAsync(ba => ba.AuthorId == authorId && ba.BookId == bookId);
	}
}
