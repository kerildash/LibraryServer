namespace Database.Services;

public class SearchService<T> : ISearchService<T>
{
	public Task<ICollection<T>> FindAsync(string query)
	{
		throw new NotImplementedException();
	}
}
