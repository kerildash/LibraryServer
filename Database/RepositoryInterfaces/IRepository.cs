using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.RepositoryInterfaces
{
	public interface IRepository<T>
	{
		T? Get(Guid id);
		ICollection<T> GetAll();
		ICollection<T> Get(string name);
		bool Exists(Guid id);
		bool Save();
	}
}
