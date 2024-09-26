using EcommerceSite.Data;
using EcommerceSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSite.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList=_db.Categories.ToList();
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
                _db.Categories.Add(ctg);
                _db.SaveChanges();
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

            Category? CategoryFromDb=_db.Categories.FirstOrDefault(x => x.Id == id);

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
                _db.Categories.Update(ctg);
                _db.SaveChanges();
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

            Category? CategoryFromDb = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _db.Categories.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
