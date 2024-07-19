using Database.RepositoryInterfaces;
using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class AuthorRepository(DataContext context, ISearchService<Author> search) : IAuthorRepository
{

	public async Task<bool> ExistsAsync(Guid id)
	{
		return await context.Authors.AnyAsync(a => a.Id == id);
	}
	public async Task<Author> GetAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException("Author not found");
		}
		return await context.Authors.FirstAsync(a => a.Id == id);
	}
	public async Task<ICollection<Author>> GetAllAsync(int pageNumber, int pageSize)
	{
		return await context.Authors.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
	}
	
	public async Task<ICollection<Author>> GetAsync(string query, int pageNumber, int pageSize)
	{
		return await search.FindAsync(query);
	}
	public async Task<ICollection<Author>> GetByBookIdAsync(Guid bookId)
	{
		return await context.BookAuthors
			.Where(e => e.BookId == bookId)
			.Select(e => e.Author).ToListAsync();
	}

	public async Task CreateAsync(Author author)
	{
		await context.AddAsync(author);
		await SaveAsync();
	}
	public async Task UpdateAsync(Author author)
	{
		context.Update(author);
		await SaveAsync();
	}
	public async Task DeleteAsync(Guid id)
	{
		if (!await ExistsAsync(id))
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
			var author = await GetAsync(id);
			context.Remove(author);
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
}
