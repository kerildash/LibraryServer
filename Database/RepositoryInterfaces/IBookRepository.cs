using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.RepositoryInterfaces
{
	public interface IBookRepository
	{
		Book Get(Guid id);
		ICollection<Book> Get(string name);
		ICollection<Book> GetAll();
	}
}
