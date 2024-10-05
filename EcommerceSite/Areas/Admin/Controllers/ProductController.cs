using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EcommerceSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _UnitOfWork = db;
            _WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List <Product> AllProduct= _UnitOfWork.Product.GetAll(IncludeProperties:"Category").ToList();
            return View(AllProduct);
        }
        public IActionResult Upsert(int? id) {
            ProductVM productVM = new ProductVM()
            {
                Product =new Product(),
                CategoryList= _UnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            };
            if(id==null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product=_UnitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
            //ViewBag.CategoryList = CategoryList;
           
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file) {
         
            if (ModelState.IsValid)
            {
                string wwwRootPath=_WebHostEnvironment.WebRootPath;
                if(file!=null)
                {
                    if (!String.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete old images
                        string oldPath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }

                    }
                    string fileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
                    string filePath=Path.Combine(wwwRootPath, @"Images\product");
                    using(var fileStream=new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl =@"\Images\product\"+ fileName;
                }
                if(productVM.Product.Id==0)
                {
                    _UnitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    _UnitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";

                }
                _UnitOfWork.Save();
                
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

        //public IActionResult Delete(int? id)
        //{
        //    if(id== null)
        //    {
        //        return NotFound();
        //    }
        //    Product? prod = _UnitOfWork.Product.Get(x => x.Id == id);
        //    if (prod == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(prod);
        //}

        //[HttpPost,ActionName("Delete")]
        //public IActionResult DeletePst(int? id)
        //{
        //    Product? prod = _UnitOfWork.Product.Get(x => x.Id == id);
        //    if (prod == null)
        //    {
        //        return NotFound();
        //    }
        //    _UnitOfWork.Product.Remove(prod);
        //    _UnitOfWork.Save();
        //    TempData["success"] = "Category deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> AllProduct = _UnitOfWork.Product.GetAll(IncludeProperties: "Category").ToList();
            return Json(new
            {
                Data = AllProduct
            });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ObjToDelete=_UnitOfWork.Product.Get(u=>u.Id==id);
            if(ObjToDelete == null) {
                return Json(new { success=false, message="Product Not Found" });
            }
            string oldPath = Path.Combine(_WebHostEnvironment.WebRootPath, ObjToDelete.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _UnitOfWork.Product.Remove(ObjToDelete);
            _UnitOfWork.Save();
            return Json(new { succuess = true, message = "Product Deleted Successfully" }); 
        }
        #endregion
    }
}
