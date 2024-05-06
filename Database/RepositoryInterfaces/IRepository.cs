
namespace Database.RepositoryInterfaces
{
	public interface IRepository<T>
	{
		Task<T> Get(Guid id);
		Task<ICollection<T>> GetAll();
		Task<ICollection<T>> Get(string name);
		Task<bool> Delete(Guid id);
		Task<bool> Update(T entity);
		Task<bool> Exists(Guid id);
		Task<bool> Save();
	}
}
