using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _UnitOfWork = db;
        }
        public IActionResult Index()
        {
            List<Category> CategoryList = _UnitOfWork.Category.GetAll().ToList();
            return View(CategoryList);
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Category ctg)
        {
            //if (ctg.Name == ctg.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "Category Name and Display Order cannot be same");
            //}
            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Add(ctg);
                _UnitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Category CategoryFromDb = _UnitOfWork.Category.Get(x => x.Id == id);

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
                _UnitOfWork.Category.Update(ctg);
                _UnitOfWork.Save();
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

            Category? CategoryFromDb = _UnitOfWork.Category.Get(x => x.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _UnitOfWork.Category.Get(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _UnitOfWork.Category.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");

        }
    }
}
