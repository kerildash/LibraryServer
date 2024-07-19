namespace Database.RepositoryInterfaces
{
	public interface IRepository<T>
	{
		Task<T> GetAsync(Guid id);
		Task<ICollection<T>> GetAllAsync(int pageNumber, int pageSize);
		Task<ICollection<T>> GetAsync(string query, int pageNumber, int pageSize);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task SaveAsync();
	}
}
