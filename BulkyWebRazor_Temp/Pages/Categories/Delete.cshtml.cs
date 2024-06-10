using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		[BindProperty] //In razor pages you have to use this annotation to bind properties that will be used in the OnPost function
		public Category Category { get; set; }
		public DeleteModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			if (id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
		}

		public IActionResult OnPost()
		{
			Category? obj = _db.Categories.Find(Category.Id);
			if (obj == null)
			{
                TempData["error"] = "Failed to Delete Category";
                return NotFound();
			}

			_db.Categories.Remove(obj);
			_db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToPage("Index");

		}
	}
}

