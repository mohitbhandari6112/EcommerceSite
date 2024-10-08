using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceSite.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;


        public CartController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;

        }
        public IActionResult Index()
        {
            var ClaimsIdentiry = (ClaimsIdentity)User.Identity;
            var UserId = ClaimsIdentiry.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM cartVM = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperties: "Product"),

            };
            foreach (var cart in cartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                cartVM.OrderTotal += (cart.Price * cart.Count);
            }
            return View(cartVM);
        }
        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _UnitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count+=1;
            _UnitOfWork.ShoppingCart.Update(cartFromDb);
            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _UnitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                _UnitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _UnitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _UnitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _UnitOfWork.ShoppingCart.Remove(cartFromDb);
            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}
