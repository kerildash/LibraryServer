using Database.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class PictureRepository(DataContext context, ISearchService<Picture> search) :
	StaticFileRepository<Picture>(context, search)
{
	private readonly DataContext _context = context;
	protected internal override DbSet<Picture> AccessTable()
	{
		return _context.Pictures;
	}
}