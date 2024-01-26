using Database.RepositoryInterfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Database.Repositories;

public class BookRepository : IBookRepository
{
	private readonly DataContext _context;
	public BookRepository(DataContext context)
	{
		_context = context;
	}
	public Book Get(Guid id)
	{

		return _context.Books.Where(b => b.Id == id).FirstOrDefault();
	}

	public ICollection<Book> Get(string title)
	{
		return _context.Books.Where(b => b.Title.Contains(title)).ToList();
	}
	public ICollection<Book> Get(Author author)
	{
		return _context.Books.Where(b => b.BookAuthors.Any(ba => ba.Author == author)).ToList();
	}


	public ICollection<Book> GetAll()
	{
		throw new NotImplementedException();
	}
}
