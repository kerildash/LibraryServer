using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.RepositoryInterfaces;

public interface ITagRepository : IRepository<Tag>
{
	public Task<ICollection<Tag>> GetByBookId(Guid bookId);
	public Task<bool> Create(Tag tag);
}
