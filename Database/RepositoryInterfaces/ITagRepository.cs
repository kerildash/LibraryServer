using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.RepositoryInterfaces;

public interface ITagRepository : IRepository<Tag>
{
	public ICollection<Tag> GetByBookId(Guid bookId);
	public bool Create(Tag tag);
}
