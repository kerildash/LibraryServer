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
		Task<T> Get(Guid id);
		Task<ICollection<T>> GetAll();
		Task<ICollection<T>> Get(string name);
		Task<bool> Delete(Guid id);
		Task<bool> Update(T entity);
		Task<bool> Exists(Guid id);
		Task<bool> Save();
	}
}
