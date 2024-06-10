using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]//If the controller is tied to an area this annotation with it's name must be used
    //Remember that all Controller classes must include the Keyword "Controller" in their name so MVC can pick them up.
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<string> invalidNames = new List<string> { "test", "testo", "testa" };
        private const string _temptDataSuccessKey = "success";
        private const string _temptDataErrorKey = "error";
        private string _productCreatedMessage = "Product Created Succesfully";
        private string _productEditedMessage = "Product Updated Succesfully";
        private string _productDeletedMessage = "Product Deleted Succesfully";
        private string _productPriceOutOfRangeMessage = "Product price is out of range, it will be clamped to it's nearest value";

		//Constructor to set the ApplicationDbContext, since the ApplicationDbContext is included in the services section of Program is not necessary to create
		//a ApplicationDbContext object, we can declare it directly on the constructor as a parameter (dependency Injection) 
		public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //In order to succesfully return the view, on the Views folder it needs to exits a Folder with the same name as the Controller without the keyword ("Product")
            //And the View must be named Index (the default name) or whatever name is used as a string parameter in the Index function
            List<Product> objectProductList = _unitOfWork.Product.GetAll().ToList();
            CreateKeys();
            return View(objectProductList); //We can pass the objectProductList as a parameter in order for it to dissplay on the view
        }

        #region Create
        public IActionResult Create()
        {
            return View(); //the returned view by default is the one the Function is named after, thus the Create function returns the Create view
        }

        //This override of the Create function is called when the user creates a new Product from the view.
        [HttpPost]//this annotation specifies this is a function that will be of type HTTPPOST
        public IActionResult Create(Product obj)
        {
            ValidationHandler(obj);

            if (ModelState.IsValid)//check if the model state has values that are valid, making sure the DisplayOrder is wihtin range for example
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();//This function saves all the changes that where done to the database, like saving the added Product from the previous line

                //TempData is a field that allows us to display notification messages for the next page render, it requires a key and a message to set (Dictionary).
                CreateTempData(_temptDataSuccessKey, _productCreatedMessage);
                //TempData[_temptDataSuccessKey] = _categoryCreatedMessage;
                return RedirectToAction("Index", "Product"); //this function allows to redirect to the specified action and controller
            }
            return View();
        }
        #endregion

        #region Edit
        public IActionResult Edit(int? id)// the id of the category we want to modify
        {
            if (id == null || id == 0)
            {
                return NotFound(); //If the id does not exists we return a NotFound page
            }

            Product? product = _unitOfWork.Product.Get(u => u.Id == id);

            //Using FirstOrDefault we can not only search for Id but other parameters like name, Find will only work by using the primary key
            //Product? category1 = _categoryRepo.Categories.FirstOrDefault(u=>u.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);//The Update funtion  allows to update the values of the database of the referenced object
                _unitOfWork.Save();
                //TempData[_temptDataSuccessKey] = _categoryEditedMessage;
                CreateTempData(_temptDataSuccessKey, _productEditedMessage);
                return RedirectToAction("Index", "Product");
            }
            return View();
        }
        #endregion

        #region Delete
        public IActionResult Delete(int? id)// the id of the category we want to modify
        {
            if (id == null || id == 0)
            {
                return NotFound(); //If the id does not exists we return a NotFound page
            }

            Product? product = _unitOfWork.Product.Get(u => u.Id == id);

            //Using FirstOrDefault we can not only search for Id but other parameters like name, Find will only work by using the primary key
            //Product? category1 = _categoryRepo.Categories.FirstOrDefault(u => u.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Since we can't override the HttpPost Delete function since the original takes the same parameters, using the ActionName annotation we can specify a
        //name that will be used to reference this function, that way we can reference it as "Delete" in posting contexts
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product? category = _unitOfWork.Product.Get(u => u.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(category);
            _unitOfWork.Save();
            //TempData[_temptDataSuccessKey] = _categoryDeletedMessage;
            CreateTempData(_temptDataSuccessKey, _productDeletedMessage);
            return RedirectToAction("Index");
        }
        #endregion

        private void CreateTempData(string key, string message)
        {
            //TempData is a field that allows us to display notification messages for the next page render, it requires a key and a message to set (Dictionary).
            TempData[key] = message;
        }

        private void CreateKeys()
        {
            //ViewBag is a dynamic property in ASP.NET MVC that allows you to pass data from the controller to the view. It's a container that stores data in a dynamic
            //manner, meaning you can add properties to it at runtime and access those properties in the view without strongly typing them, we'll use it to create and store
            //the keys that will be used for tempData messsages
            ViewBag.TempDataSuccessKey = _temptDataSuccessKey;
            ViewBag.TempDataErrorKey = _temptDataErrorKey;
        }

        #region CreateValidationFunctions
        //Function that encapsulates all other validation Functions, to only have to call one to check them all
        private void ValidationHandler(Product product)
        {
            int minPrice = product.MinPrice;
            int maxPrice = product.MaxPrice;

			if (string.IsNullOrEmpty(product.Title))
            {
                ModelState.AddModelError("title", "No title was introduced");
                return;
            }

			if (string.IsNullOrEmpty(product.Description))
			{
				ModelState.AddModelError("Description", "No Description was introduced");
				return;
			}

			if (string.IsNullOrEmpty(product.ISBN))
			{
				ModelState.AddModelError("ISBN", "No ISBN was introduced");
				return;
			}

			if (string.IsNullOrEmpty(product.Author))
			{
				ModelState.AddModelError("Author", "No Author was introduced");
				return;
			}

			product.ListPrice = ClampPriceWithinRange(product.ListPrice, minPrice, maxPrice);
			product.Price = ClampPriceWithinRange(product.Price, minPrice, maxPrice);
			product.Price50 = ClampPriceWithinRange(product.Price50, minPrice, maxPrice);
			product.Price100 = ClampPriceWithinRange(product.Price100, minPrice, maxPrice);

			InvalidNameValidation(product.Title);
        }

        private void InvalidNameValidation(string name)
        {
            for (int i = 0; i < invalidNames.Count; i++)
            {
                if (string.Equals(name.ToLower(), invalidNames[i].ToLower()))
                {
                    //If an Add Model Error has a empty key and the view contains an asp-validation-summary tag then the error will be displayed there.
                    ModelState.AddModelError("", $"{name} is not a valid entry name");
                    return;
                }
            }
        }

        private double ClampPriceWithinRange(double value, double minValue, double maxValue)
        {
            if(!(value > minValue && value < maxValue)) //if value is not in range
            {
				ModelState.AddModelError(value.ToString(), _productPriceOutOfRangeMessage);
				return Math.Clamp(value, minValue, maxValue);
			}

            return value;
		}
        #endregion
    }
}
