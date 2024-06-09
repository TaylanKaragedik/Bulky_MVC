using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;   
        public ProductController(IProductRepository db, ICategoryRepository dbCategory, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = db;
            _categoryRepo = dbCategory;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _productRepo.GetAll(includeProperties: "Category").ToList();
            return View(objProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //Create
                return View(productViewModel);
            }
            else
            {
                //Update
                productViewModel.Product = _productRepo.Get(u => u.Id == id);
				return View(productViewModel);
			}
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productViewModel, IFormFile? file) 
        {
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productViewModel.Product.Id == 0)
                {
                    _productRepo.Add(productViewModel.Product);

                }
                else
                {
                    _productRepo.Update(productViewModel.Product);
                }
                _productRepo.Save();
                TempData["success"] = "The product created succesfully";
                return RedirectToAction("Index");
            }
            else
            {
				productViewModel.CategoryList = _categoryRepo.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
				return View(productViewModel);
			}
            
        }
        public IActionResult DeletePost(int? id)
        {
            Product? obj = _productRepo.Get(u => u.Id == id);
            if (obj == null)
            { return NotFound(); }
            _productRepo.Remove(obj);
            _productRepo.Save();
            TempData["success"] = "The deleted successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Product> objProductList = _productRepo.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
		public IActionResult Delete(int? id)
		{
            var productToBeDeleted = _productRepo.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {

                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            _productRepo.Remove(productToBeDeleted);
            _productRepo.Save();

			return Json(new { success = true, message = "Delete Succesful" });
		}
		#endregion
	}
}
