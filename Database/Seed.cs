using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Database;

public class Seed
{
	private readonly DataContext _dataContext;
	public Seed(DataContext context)
	{
		_dataContext = context;
	}
	public void SeedDataContext()
	{
		if (!_dataContext.BookAuthors.Any())
		{
			var bookAuthors = new List<BookAuthor>()
			{
				new BookAuthor()
				{
					Book = new Book()
					{
						Title = "Title",
						Description = "Description"
					},
					Author = new Author()
					{
						Name = "Author",
						Bio = "Bio"
					}
				},
				new BookAuthor()
				{
					Book = new Book()
					{
						Title = "Title2",
						Description = "Description2"
					},
					Author = new Author()
					{
						Name = "Author2",
						Bio = "Bio2"
					}
				},
			};
			_dataContext.BookAuthors.AddRange(bookAuthors);
			_dataContext.SaveChanges();
		}
	}
}
