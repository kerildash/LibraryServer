using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Database.RepositoryInterfaces
{
	public interface IStaticFileRepository : IRepository<StaticFile>
	{
		Task<ICollection<StaticFile>> GetByHolderId(Guid parentId);
		Task<bool> Create(StaticFile document);
	}
}
