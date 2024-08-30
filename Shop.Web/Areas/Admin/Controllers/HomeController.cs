using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Web.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class HomeController(IOrderService orderService) : AdminBaseController
    {

        #region constractor
        private readonly IOrderService _orderService = orderService;
        #endregion

        #region dashboard
        public async Task<IActionResult> Index()
        {
            ViewData["ResultOrder"] = await _orderService.GetResultOrder();
            return View();
        }
        #endregion
    }
}
