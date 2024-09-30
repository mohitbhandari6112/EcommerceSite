using Ecommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}
