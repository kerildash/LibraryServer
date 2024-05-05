using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories
{
	public class DocumentRepository(DataContext context) : IDocumentRepository
	{
		public async Task<bool> Exists(Guid id)
		{
			return await context.Documents.AnyAsync(a => a.Id == id);
		}
		public async Task<StaticFile> Get(Guid id)
		{
			if (!await Exists(id))
			{
				throw new ArgumentException("Document not found");
			}
			return await context.Documents.FirstOrDefaultAsync(d => d.Id == id) ?? throw new NullReferenceException();
		}

		public async Task<ICollection<StaticFile>> Get(string name)
		{
			throw new NotImplementedException();
		}
		public async Task<ICollection<StaticFile>> GetAll()
		{
			return (ICollection<StaticFile>)await context.Documents.ToListAsync();
		}
		public async Task<ICollection<StaticFile>> GetByHolderId(Guid HolderId)
		{
			return (ICollection<StaticFile>)await context.Documents.Where(d => d.HolderId == HolderId).ToListAsync();
		}
		public async Task<bool> Create(StaticFile document)
		{
			await context.AddAsync(document);
			return await Save();
		}

		public async Task<bool> Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		

		
		public async Task<bool> Save()
		{
			try
			{
				return await context.SaveChangesAsync() > 0
					? true
					: throw new InvalidOperationException("Nothing to save");
			}
			catch
			{
				throw;
			}
		}

		public async Task<bool> Update(StaticFile entity)
		{
			throw new NotImplementedException();
		}
	}
}
