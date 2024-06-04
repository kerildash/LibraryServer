using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class DocumentRepository(DataContext context, ISearchService<Document> search) :
	StaticFileRepository<Document>(context, search)
{
	private readonly DataContext _context = context;
	protected internal override DbSet<Document> AccessTable()
	{
		return _context.Documents;
	}
}
