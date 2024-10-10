using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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
				OrderHeader = new()

			};
			foreach (var cart in cartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(cartVM);
		}
		public IActionResult Summary()
		{
			var ClaimsIdentiry = (ClaimsIdentity)User.Identity;
			var UserId = ClaimsIdentiry.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM cartVM = new()
			{
				ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperties: "Product"),
				OrderHeader = new()

			};
			cartVM.OrderHeader.ApplicationUser = _UnitOfWork.ApplicationUser.Get(u => u.Id == UserId);
			foreach (var cart in cartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(cartVM);
		}

		[HttpPost]
		public IActionResult Summary(ShoppingCartVM shoppingCartVM)
		{
			var ClaimsIdentiry = (ClaimsIdentity)User.Identity;
			var UserId = ClaimsIdentiry.FindFirst(ClaimTypes.NameIdentifier).Value;
			ShoppingCartVM cartVM = new()
			{
				ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId, IncludeProperties: "Product"),
				OrderHeader = shoppingCartVM.OrderHeader

			};
			cartVM.OrderHeader.ApplicationUserId = UserId;
			cartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ApplicationUser applicationUser = _UnitOfWork.ApplicationUser.Get(u => u.Id == UserId);

			foreach (var cart in cartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				cartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				cartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
				cartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				cartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}

			_UnitOfWork.OrderHeader.Add(cartVM.OrderHeader);
			_UnitOfWork.Save();
			foreach (var cart in cartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.Product.Id,
					OrderHeaderId = cartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count

				};
				_UnitOfWork.OrderDetail.Add(orderDetail);
				_UnitOfWork.Save();
			}
			//logic for payment
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				var domain = "https://localhost:7239/";
				
				var options = new Stripe.Checkout.SessionCreateOptions
				{
					SuccessUrl =domain+$"customer/cart/OrderConfirmation/?id={cartVM.OrderHeader.Id}",
					CancelUrl=domain+"customer/cart/index",
					LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
					Mode = "payment",
				};
				foreach(var item in cartVM.ShoppingCartList)
				{
					var SessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title
							}
						},
						Quantity = item.Count

					};
					options.LineItems.Add(SessionLineItem);
				}
				var service = new Stripe.Checkout.SessionService();
				Session session=service.Create(options);
				_UnitOfWork.OrderHeader.UpdateStripePaymentId(cartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_UnitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);


			}
			return RedirectToAction(nameof(OrderConfirmation), new { id = cartVM.OrderHeader.Id });
		}
		public IActionResult OrderConfirmation(int id)

		{
			OrderHeader orderHeader = _UnitOfWork.OrderHeader.Get(u => u.Id == id, IncludeProperties: "ApplicationUser");
			if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);
				if (session.PaymentStatus.ToLower() == "paid")
				{
					_UnitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					_UnitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_UnitOfWork.Save();
				}

			}
			List<ShoppingCart> cartList = _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_UnitOfWork.ShoppingCart.RemoveRange(cartList);
			_UnitOfWork.Save();
			return View(id);
		}
		public IActionResult Plus(int cartId)
		{
			var cartFromDb = _UnitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			cartFromDb.Count += 1;
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
