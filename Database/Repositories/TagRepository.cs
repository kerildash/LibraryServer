using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class TagRepository(DataContext context) : ITagRepository
{

	public async Task<bool> Exists(Guid id)
	{
		return await context.Tags.AnyAsync(a => a.Id == id);
	}
	public async Task<Tag> Get(Guid id)
	{
		if (!await Exists(id))
		{
			throw new ArgumentException("Tag not found");
		}
		return await context.Tags.FirstOrDefaultAsync(a => a.Id == id) ?? throw new NullReferenceException();
	}
	public async Task<ICollection<Tag>> GetAll()
	{
		return await context.Tags.ToListAsync();
	}
	public async Task<ICollection<Tag>> Get(string name)
	{
		throw new NotImplementedException();
	}
	public async Task<ICollection<Tag>> GetByBookId(Guid bookId)
	{
		return await context.BookTags
			.Where(e => e.BookId == bookId)
			.Select(e => e.Tag).ToListAsync();
	}

	public async Task<bool> Create(Tag tag)
	{
		await context.AddAsync(tag);
		return await Save();
	}
	public async Task<bool> Update(Tag tag)
	{
		context.Update(tag);
		return await Save();
	}
	public async Task<bool> Delete(Guid id)
	{
		if (!await Exists(id))
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
		try
		{
			var tag = await Get(id);
			context.Remove(tag);
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
