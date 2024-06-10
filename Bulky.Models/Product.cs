using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ISBN { get; set; }
		public string Author {  get; set; }
		private const int _minPrice = 1;
		private const int _maxPrice = 1000;

		public int MinPrice { get { return _minPrice; } }
		public int MaxPrice { get { return _maxPrice; } }

		[Required]
		[Display(Name = "List Price")] //annotation so the name on the view is spaced instead  of using the cammelNotation one
		[Range(_minPrice, _maxPrice)]
		public double ListPrice { get; set; }

		[Required]
		[Display(Name = "Price for 1-50")]
		[Range(_minPrice, _maxPrice)]
		public double Price { get; set; }
		[Required]
		[Display(Name = "Price for 50+")]
		[Range(_minPrice, _maxPrice)]
		public double Price50 { get; set; }

		[Required]
		[Display(Name = "Price for 100+")]
		[Range(_minPrice, _maxPrice)]
		public double Price100 { get; set; }

		//public List<double> GetPricesAsList()
		//{
		//	List<double> priceList = [ListPrice, Price, Price50, Price100];
		//	return priceList;
		//}
	}
}
