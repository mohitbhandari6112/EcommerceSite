using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EcommerceSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public ProductController(IUnitOfWork db)
        {
            _UnitOfWork = db;
        }
        public IActionResult Index()
        {
            List <Product> AllProduct= _UnitOfWork.Product.GetAll().ToList();
   

            return View(AllProduct);
        }
        public IActionResult Add() {
            ProductVM productVM = new ProductVM()
            {
                Product =new Product(),
                CategoryList= _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            };
            //ViewBag.CategoryList = CategoryList;
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Add(ProductVM productVM) {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Add(productVM.Product);
                _UnitOfWork.Save();
                TempData["success"] = "Product created successfully";
                //return RedirectToAction("Index");
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return View(productVM);
        }

        public IActionResult Edit(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            Product Prod=_UnitOfWork.Product.Get(u=>u.Id==id);
            if(Prod== null)
            {
                return NotFound();
            }
            return View(Prod);
        }
        [HttpPost]
        public IActionResult Edit(Product prod)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Update(prod);
                _UnitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        public IActionResult Delete(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            Product? prod = _UnitOfWork.Product.Get(x => x.Id == id);
            if (prod == null)
            {
                return NotFound();
            }
            return View(prod);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePst(int? id)
        {
            Product? prod = _UnitOfWork.Product.Get(x => x.Id == id);
            if (prod == null)
            {
                return NotFound();
            }
            _UnitOfWork.Product.Remove(prod);
            _UnitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
