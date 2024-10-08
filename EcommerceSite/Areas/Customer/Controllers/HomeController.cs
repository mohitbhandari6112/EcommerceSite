using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product= _UnitOfWork.Product.Get(u => u.Id == productId, IncludeProperties: "Category"),
                ProductId=productId,
                Count=1
            };   
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart Cart)
        {
            var ClaimsIdentiry = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentiry.FindFirst(ClaimTypes.NameIdentifier).Value;
            Cart.ApplicationUserId = UserId;

            ShoppingCart CartFromDb=_UnitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == UserId&&u.ProductId==Cart.ProductId);
            if (CartFromDb != null)
            {
                CartFromDb.Count += Cart.Count;
                _UnitOfWork.ShoppingCart.Update(CartFromDb);
            }
            else
            {

                _UnitOfWork.ShoppingCart.Add(Cart);

            }
            TempData["success"] = "Cart updated successfully";
            _UnitOfWork.Save();

            return RedirectToAction(nameof(Index));
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
