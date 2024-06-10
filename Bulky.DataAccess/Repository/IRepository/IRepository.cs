using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{

	//<T> where T : class means the interface doesn't has an sspecific type, it will be implemented as long as T references a class.
	public interface IRepository<T> where T : class
	{
		IEnumerable<T> GetAll();

		T Get(Expression<Func<T, bool>> filter); 
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entity);
	}
}
