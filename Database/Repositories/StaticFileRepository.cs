using Database.RepositoryInterfaces;
using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public abstract class StaticFileRepository<T>
	(DataContext context, ISearchService<T> search)
	: IStaticFileRepository<T> where T : StaticFile
{
	private readonly string TypeName = typeof(T).Name.ToLower();
	public async Task<bool> ExistsAsync(Guid id)
	{
		return await AccessTable().AnyAsync(a => a.Id == id);
	}
	public async Task<T> GetAsync(Guid id)
	{
		if (!await ExistsAsync(id))
		{
			throw new ArgumentException($"{TypeName} not found");
		}
		return await AccessTable().FirstAsync(d => d.Id == id);
	}

	public async Task<ICollection<T>> GetAsync(string query)
	{
		return await search.FindAsync(query);
	}
	public async Task<ICollection<T>> GetAllAsync()
	{
		return await AccessTable().ToListAsync();
	}
	public async Task<ICollection<T>> GetByHolderIdAsync(Guid HolderId)
	{
		return await AccessTable().Where(d => d.HolderId == HolderId).ToListAsync();
	}
	public async Task CreateAsync(T staticFile)
	{
		await context.AddAsync(staticFile);
		await SaveAsync();
	}

	public async Task DeleteAsync(Guid id)
	{
		var staticFile = await GetAsync(id);

		bool holderExists =
		await context.Books.AnyAsync(b => b.Id == staticFile.HolderId) ||
		await context.Authors.AnyAsync(b => b.Id == staticFile.HolderId);

		if (staticFile.HolderId is not null && holderExists)
		{
			throw new InvalidOperationException
			($"There is entity with id {staticFile.HolderId} holding this {TypeName} with id {staticFile.Id}");
		}

		context.Remove(staticFile);
		await SaveAsync();
	}

	public async Task SaveAsync()
	{
		await context.SaveChangesAsync();
	}

	public async Task UpdateAsync(T document)
	{
		context.Update(document);
		await SaveAsync();
	}
	protected internal abstract DbSet<T> AccessTable();
}
