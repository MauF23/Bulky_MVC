using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
	[BindProperties]
    public class EditModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		public Category Category { get; set; }
		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int?id)
		{
			if(id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
		}

		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category);//The Update funtion  allows to update the values of the database of the referenced object
				_db.SaveChanges();
                TempData["success"] = "Category Edited Successfully";
                return RedirectToPage("Index");
			}
            TempData["error"] = "Failed to Edit Category";
            return Page();
		}
	}
}
