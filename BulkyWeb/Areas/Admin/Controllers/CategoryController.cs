using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]//If the controller is tied to an area this annotation with it's name must be used
    //Remember that all Controller classes must include the Keyword "Controller" in their name so MVC can pick them up.
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private List<string> invalidNames = new List<string> { "test", "testo", "testa" };
        private const string _temptDataSuccessKey = "success";
        private const string _temptDataErrorKey = "error";
        private string _categoryCreatedMessage = "Category Created Succesfully";
        private string _categoryEditedMessage = "Category Updated Succesfully";
        private string _categoryDeletedMessage = "Category Deleted Succesfully";

        //Constructor to set the ApplicationDbContext, since the ApplicationDbContext is included in the services section of Program is not necessary to create
        //a ApplicationDbContext object, we can declare it directly on the constructor as a parameter (dependency Injection) 
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            //In order to succesfully return the view, on the Views folder it needs to exits a Folder with the same name as the Controller without the keyword ("Category")
            //And the View must be named Index (the default name) or whatever name is used as a string parameter in the Index function
            List<Category> objectCategoryList = _unitOfWork.Category.GetAll().ToList();
            CreateKeys();
            return View(objectCategoryList); //We can pass the objectCategoryList as a parameter in order for it to dissplay on the view
        }

        #region Create
        public IActionResult Create()
        {
            return View(); //the returned view by default is the one the Function is named after, thus the Create function returns the Create view
        }

        //This override of the Create function is called when the user creates a new Category from the view.
        [HttpPost]//this annotation specifies this is a function that will be of type HTTPPOST
        public IActionResult Create(Category obj)
        {
            ValidationHandler(obj.Name, obj.DisplayOrder);

            if (ModelState.IsValid)//check if the model state has values that are valid, making sure the DisplayOrder is wihtin range for example
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();//This function saves all the changes that where done to the database, like saving the added Category from the previous line

                //TempData is a field that allows us to display notification messages for the next page render, it requires a key and a message to set (Dictionary).
                CreateTempData(_temptDataSuccessKey, _categoryCreatedMessage);
                //TempData[_temptDataSuccessKey] = _categoryCreatedMessage;
                return RedirectToAction("Index", "Category"); //this function allows to redirect to the specified action and controller
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

            Category? category = _unitOfWork.Category.Get(u => u.Id == id);

            //Using FirstOrDefault we can not only search for Id but other parameters like name, Find will only work by using the primary key
            //Category? category1 = _categoryRepo.Categories.FirstOrDefault(u=>u.Id==id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);//The Update funtion  allows to update the values of the database of the referenced object
                _unitOfWork.Save();
                //TempData[_temptDataSuccessKey] = _categoryEditedMessage;
                CreateTempData(_temptDataSuccessKey, _categoryEditedMessage);
                return RedirectToAction("Index", "Category");
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

            Category? category = _unitOfWork.Category.Get(u => u.Id == id);

            //Using FirstOrDefault we can not only search for Id but other parameters like name, Find will only work by using the primary key
            //Category? category1 = _categoryRepo.Categories.FirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //Since we can't override the HttpPost Delete function since the original takes the same parameters, using the ActionName annotation we can specify a
        //name that will be used to reference this function, that way we can reference it as "Delete" in posting contexts
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? category = _unitOfWork.Category.Get(u => u.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            //TempData[_temptDataSuccessKey] = _categoryDeletedMessage;
            CreateTempData(_temptDataSuccessKey, _categoryDeletedMessage);
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
        private void ValidationHandler(string name, int displayOrder)
        {
            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "No name was introduced");
                return;
            }
            NameDisplayOrderValitation(name, displayOrder);
            InvalidNameValidation(name);
        }

        //This Function is a custom validation, prevents the name from being the same as the displayOrder
        private void NameDisplayOrderValitation(string name, int displayOrder)
        {
            if (string.Equals(name.ToLower(), displayOrder.ToString().ToLower()))
            {
                //AddModelError allows to define a custom error message, the first parameter is the key which must match one of the variables of the Model/Class
                //in this case the key is name, so the error will be displayed in the field that has that variable as a reference, it's not case sensitive
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name");
            }
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
        #endregion
    }
}
