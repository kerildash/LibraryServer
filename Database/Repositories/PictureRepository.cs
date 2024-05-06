using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories
{
	public class PictureRepository(DataContext context) : IPictureRepository
	{
		public async Task<bool> Exists(Guid id)
		{
			return await context.Pictures.AnyAsync(a => a.Id == id);
		}
		public async Task<StaticFile> Get(Guid id)
		{
			if (!await Exists(id))
			{
				throw new ArgumentException("Document not found");
			}
			return await context.Pictures.FirstOrDefaultAsync(d => d.Id == id) ?? throw new NotImplementedException();
		}
		public async Task<ICollection<StaticFile>> Get(string name)
		{
			throw new NotImplementedException();
		}
		public async Task<ICollection<StaticFile>> GetAll()
		{
			return (ICollection<StaticFile>)await context.Pictures.ToListAsync();
		}
		public async Task<ICollection<StaticFile>> GetByHolderId(Guid parentId)
		{
			return (ICollection<StaticFile>)await context.Pictures.Where(d => d.HolderId == parentId).ToListAsync();
		}
		public async Task<bool> Create(StaticFile picture)
		{
			await context.AddAsync(picture);
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
