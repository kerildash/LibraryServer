using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class AuthorRepository(DataContext context) : IAuthorRepository
{

	public async Task<bool> Exists(Guid id)
	{
		return await context.Authors.AnyAsync(a => a.Id == id);
	}
	public async Task<Author> Get(Guid id)
	{
		if (!await Exists(id))
		{
			throw new ArgumentException("Author not found");
		}
		return await context.Authors.FirstOrDefaultAsync(a => a.Id == id) ?? throw new NullReferenceException();
	}
	public async Task<ICollection<Author>> GetAll()
	{
		return await context.Authors.ToListAsync();
	}
	
	public async Task<ICollection<Author>> Get(string name)
	{
		throw new NotImplementedException();
	}
	public async Task<ICollection<Author>> GetByBookId(Guid bookId)
	{
		return await context.BookAuthors
			.Where(e => e.BookId == bookId)
			.Select(e => e.Author).ToListAsync();
	}

	public async Task<bool> Create(Author author)
	{
		await context.AddAsync(author);
		return await Save();
	}
	public async Task<bool> Update(Author author)
	{
		context.Update(author);
		return await Save();
	}
	public async Task<bool> Delete(Guid id)
	{
		if (!await Exists(id))
		{
			throw new ArgumentException($"Author ID: \"{id}\" does not exist");
		}
		if (context.BookAuthors.Where(ba => ba.AuthorId == id).Any())
		{
			throw new InvalidOperationException
				($"Deletion not allowed: " +
				$"there are 1 or more books related with this author." +
				$"Delete them and try again.");
		}
		try
		{
			var author = await Get(id);
			context.Remove(author);
			return await Save();
		}
		catch
		{
			throw;
		}
	}
	public async Task<bool> Save()
	{
		return (await context.SaveChangesAsync() > 0) ? true : false;
	}
}
