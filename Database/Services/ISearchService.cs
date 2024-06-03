using Domain.Models;

namespace Database.Services;

internal interface ISearchService<T> where T : StaticFile
{
	Task<T> FindAsync(string query);
}
