using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceSite.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork UnitOfWork)
        {
            _logger = logger;
            _UnitOfWork=UnitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products=_UnitOfWork.Product.GetAll(IncludeProperties:"Category");
            return View(products);
        }
        public IActionResult Details(int id)
        {
            Product product = _UnitOfWork.Product.Get(u=>u.Id==id,IncludeProperties: "Category");
            return View(product);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
