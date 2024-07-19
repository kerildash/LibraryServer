using Database.RepositoryInterfaces;
using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class TagRepository(DataContext context, ISearchService<Tag> search) : ITagRepository
{

	public async Task<bool> ExistsAsync(Guid id)
	{
		return await context.Tags.AnyAsync(a => a.Id == id);
	}
	public async Task<Tag> GetAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException("Tag not found");
		}
		return await context.Tags.FirstAsync(a => a.Id == id);
	}
	public async Task<ICollection<Tag>> GetAllAsync(int pageNumber, int pageSize)
	{
		return await context.Tags.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
	}
	public async Task<ICollection<Tag>> GetAsync(string query, int pageNumber, int pageSize)
	{
		return await search.FindAsync(query);
	}
	public async Task<ICollection<Tag>> GetByBookIdAsync(Guid bookId)
	{
		return await context.BookTags
			.Where(e => e.BookId == bookId)
			.Select(e => e.Tag).ToListAsync();
	}

	public async Task CreateAsync(Tag tag)
	{
		await context.AddAsync(tag);
		await SaveAsync();
	}
	public async Task UpdateAsync(Tag tag)
	{
		context.Update(tag);
		await SaveAsync();
	}
	public async Task DeleteAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException($"Tag ID: \"{id}\" does not exist");
		}
		if (await context.BookTags.Where(ba => ba.TagId == id).AnyAsync())
		{
			throw new InvalidOperationException
				($"Deletion not allowed: " +
				$"there are 1 or more books related with this tag." +
				$"Delete them and try again.");
		}
		var tag = await GetAsync(id);
		context.Remove(tag);
		await SaveAsync();
	}
	public async Task SaveAsync()
	{
		await context.SaveChangesAsync();
	}
}
