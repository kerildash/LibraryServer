using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class AuthorRepository(DataContext context) : IAuthorRepository
{

	public bool Exists(Guid id)
	{
		return context.Authors.Any(a => a.Id == id);
	}
	public Author Get(Guid id)
	{
		if (!Exists(id))
		{
			throw new ArgumentException("Author not found");
		}
		return context.Authors.FirstOrDefault(a => a.Id == id);
	}
	public ICollection<Author> GetAll()
	{
		return context.Authors.ToList();
	}
	public ICollection<Author> Get(string name)
	{
		throw new NotImplementedException();
	}
	public ICollection<Author> GetByBookId(Guid bookId)
	{
		return context.BookAuthors
			.Where(e => e.BookId == bookId)
			.Select(e => e.Author).ToList();
	}

	public bool Create(Author author)
	{
		context.Add(author);
		return Save();
	}
	public bool Update(Author author)
	{
		context.Update(author);
		return Save();
	}
	public bool Delete(Guid id)
	{
		if (!Exists(id))
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
			var author = Get(id);
			context.Remove(author);
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
