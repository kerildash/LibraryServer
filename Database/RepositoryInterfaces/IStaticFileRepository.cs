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
		ICollection<StaticFile> GetByHolderId(Guid parentId);
		bool Create(StaticFile document);
	}
}
