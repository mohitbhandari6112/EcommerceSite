using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models.ViewModel;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public CompanyController(IUnitOfWork db)
        {
            _UnitOfWork = db;
        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult Upsert(int? id)
        {

            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                var Company = _UnitOfWork.Company.Get(u => u.Id == id);

                return View(Company);
            }
            //ViewBag.CategoryList = CategoryList;

        }

        [HttpPost]
        public IActionResult Upsert(Company ComObj)
        {
            if (ModelState.IsValid)
            {
                if (ComObj.Id == 0)
                {
                    _UnitOfWork.Company.Add(ComObj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _UnitOfWork.Company.Update(ComObj);
                    TempData["success"] = "Company updated successfully";

                }
                _UnitOfWork.Save();

                //return RedirectToAction("Index");
                return RedirectToAction("Index");
            }
            return View(ComObj);
        }

        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = _UnitOfWork.Company.GetAll().ToList();
            return Json(new
            {
                Data = Companies
            });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ObjToDelete = _UnitOfWork.Company.Get(u => u.Id == id);
            if (ObjToDelete == null)
            {
                return Json(new { success = false, message = "Company Not Found" });
            }
            _UnitOfWork.Company.Remove(ObjToDelete);
            _UnitOfWork.Save();
            return Json(new { succuess = true, message = "Company Deleted Successfully" });
        }
        #endregion

    }
}
