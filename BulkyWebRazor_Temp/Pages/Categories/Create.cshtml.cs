using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
	public class CreateModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		[BindProperty] //In razor pages you have to use this annotation to bind properties that will be used in the OnPost function
		public Category Category { get; set; }
		public CreateModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet()
		{

		}

		public IActionResult OnPost()
		{
			_db.Categories.Add(Category);
			_db.SaveChanges();
			TempData["success"] = "Category Created Successfully";

			//On Razor Pages we don't return a View, we redirect to a page with this function
			return RedirectToPage("Index");
		}
	}

}
