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
		public bool Exists(Guid id)
		{
			return context.Documents.Any(a => a.Id == id);
		}
		public StaticFile Get(Guid id)
		{
			if (!Exists(id))
			{
				throw new ArgumentException("Document not found");
			}
			return context.Documents.FirstOrDefault(d => d.Id == id);
		}
		public ICollection<StaticFile> Get(string name)
		{
			throw new NotImplementedException();
		}
		public ICollection<StaticFile> GetAll()
		{
			return (ICollection<StaticFile>)context.Documents.ToList();
		}
		public ICollection<StaticFile> GetByHolderId(Guid HolderId)
		{
			return (ICollection<StaticFile>)context.Documents.Where(d => d.HolderId == HolderId).ToList();
		}
		public bool Create(StaticFile document)
		{
			context.Add(document);
			return Save();
		}

		public bool Delete(Guid id)
		{
			throw new NotImplementedException();
		}

		

		
		public bool Save()
		{
			try
			{
				return context.SaveChanges() > 0
					? true
					: throw new InvalidOperationException("Nothing to save");
			}
			catch
			{
				throw;
			}
		}

		public bool Update(StaticFile entity)
		{
			throw new NotImplementedException();
		}
	}
}
