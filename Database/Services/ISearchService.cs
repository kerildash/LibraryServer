namespace Database.Services;

public interface ISearchService<T>
{
	Task<ICollection<T>> FindAsync(string query);
}
