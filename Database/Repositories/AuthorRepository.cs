using Database.RepositoryInterfaces;
using Domain.Models;

namespace Database.Repositories;

public class AuthorRepository(DataContext context) : IAuthorRepository
{

	public bool Exists(Guid id)
	{
		return context.Authors.Any(a => a.Id == id);
	}
	public Author? Get(Guid id)
	{
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
	public bool Save()
	{
		return (context.SaveChanges() > 0) ? true : false;
	}
}
