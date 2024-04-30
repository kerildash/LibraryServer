using Database.RepositoryInterfaces;
using Domain.Models;

namespace Database.Repositories;

public class TagRepository(DataContext context) : ITagRepository
{

	public bool Exists(Guid id)
	{
		return context.Tags.Any(a => a.Id == id);
	}
	public Tag Get(Guid id)
	{
		if (!Exists(id))
		{
			throw new ArgumentException("Tag not found");
		}
		return context.Tags.FirstOrDefault(a => a.Id == id);
	}
	public ICollection<Tag> GetAll()
	{
		return context.Tags.ToList();
	}
	public ICollection<Tag> Get(string name)
	{
		throw new NotImplementedException();
	}
	public ICollection<Tag> GetByBookId(Guid bookId)
	{
		return context.BookTags
			.Where(e => e.BookId == bookId)
			.Select(e => e.Tag).ToList();
	}

	public bool Create(Tag tag)
	{
		context.Add(tag);
		return Save();
	}
	public bool Update(Tag tag)
	{
		context.Update(tag);
		return Save();
	}
	public bool Delete(Guid id)
	{
		if (!Exists(id))
		{
			throw new ArgumentException($"Tag ID: \"{id}\" does not exist");
		}
		if (context.BookTags.Where(ba => ba.TagId == id).Any())
		{
			throw new InvalidOperationException
				($"Deletion not allowed: " +
				$"there are 1 or more books related with this tag." +
				$"Delete them and try again.");
		}
		try
		{
			var tag = Get(id);
			context.Remove(tag);
			return Save();
		}
		catch
		{
			throw;
		}
	}
	public bool Save()
	{
		return (context.SaveChanges() > 0) ? true : false;
	}
}
