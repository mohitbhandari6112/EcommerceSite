using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using Ecommerce.Models.ViewModel;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceSite.Areas.Admin.Controllers
{
    [Area("admin")]


    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public OrderController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int orderId)

        {
            OrderVM orderVM = new()
            {
                OrderHeader = _UnitOfWork.OrderHeader.Get(u => u.Id == orderId, IncludeProperties: "ApplicationUser"),
                OrderDetail = _UnitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId,IncludeProperties:"Product")
            };
            return View(orderVM);
        }

        #region API calls
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders = _UnitOfWork.OrderHeader.GetAll(IncludeProperties: "ApplicationUser").ToList();
            switch (status)
            {
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }
            return Json(new
            {
                Data = orderHeaders
            });
        }
        #endregion
    }
}
