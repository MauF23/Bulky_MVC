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
	internal class ProductRepository : Repository<Product>, IProductRepository
	{
		private ApplicationDbContext _db;

		//the : base(db) references the base class, since the reference _db is created after the class we need to reference the base to avoid a notArgument error
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Product obj)
		{
			_db.Products.Update(obj);
		}
	}
}
