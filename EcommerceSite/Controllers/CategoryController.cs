using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSite.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _CategoryRepo;
        public CategoryController(ICategoryRepository db)
        {
            _CategoryRepo = db;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = (List<Category>)_CategoryRepo.GetAll();
            return View(CategoryList);
        }
        public  IActionResult Add() {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category ctg)
        {
            //if (ctg.Name == ctg.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "Category Name and Display Order cannot be same");
            //}
            if (ModelState.IsValid) {
                _CategoryRepo.Add(ctg);
                _CategoryRepo.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();
           
        }

        public IActionResult Edit(int id)
        {
            if ( id == 0)
            {
                return NotFound();
            }

            Category CategoryFromDb=_CategoryRepo.Get(x => x.Id == id);

            //Category CategoryFromDb1=_db.Categories.Find(id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category ctg)
        {
            if (ModelState.IsValid)
            {
                 _CategoryRepo.Update(ctg);
                _CategoryRepo.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Category? CategoryFromDb = _CategoryRepo.Get(x => x.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _CategoryRepo.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _CategoryRepo.Remove(obj);
            _CategoryRepo.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
