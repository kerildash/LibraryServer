namespace Database.RepositoryInterfaces
{
	public interface IRepository<T>
	{
		Task<T> GetAsync(Guid id);
		Task<ICollection<T>> GetAllAsync();
		Task<ICollection<T>> GetAsync(string prompt);
		Task DeleteAsync(Guid id);
		Task<bool> ExistsAsync(Guid id);
		Task SaveAsync();
	}
}
