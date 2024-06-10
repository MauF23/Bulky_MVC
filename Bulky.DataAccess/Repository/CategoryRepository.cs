using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository 
	{ 

		private ApplicationDbContext _db;

		//the : base(db) references the base class, since the reference _db is created after the class we need to reference the base to avoid a notArgument error
		public CategoryRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		//public void Save()
		//{
		//	//_db.SaveChanges();
		//}

		public void Update(Category obj)
		{
			_db.Categories.Update(obj);
		}
	}
}
