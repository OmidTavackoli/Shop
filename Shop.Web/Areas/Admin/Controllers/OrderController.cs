using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.ViewModels.Admin.Order;
using Shop.Web.Extentions;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class OrderController(IOrderService orderService) : AdminBaseController
    {
        #region constractor
        private readonly IOrderService _orderService = orderService;
        #endregion

        #region filter-orders
        public async Task<IActionResult> FilterOrders(FilterOrdersViewModel filterOrders)
        {
            return View(await _orderService.FilterOrders(filterOrders));
        }
        #endregion

        #region change order-state
        public async Task<IActionResult> ChangeStateToSent(long orderId)
        {
            var result = await _orderService.ChangeStateToSent(orderId);
            if (result)
            {
                return JsonResponseStatus.Success();
            }

            return JsonResponseStatus.Error();

        }
        #endregion

        #region OrderDetail
        public async Task<IActionResult> OrderDetail(long orderId)
        {
            var data = await _orderService.GetOrderDetail(orderId);
            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }
        #endregion
    }
}
