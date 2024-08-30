using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shop.Application.Interfaces;
using Shop.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    public class HomeController : SiteBaseController
    {
        #region constarctor
        private readonly IProductService _productService;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            ViewData["LastProducts"] = await _productService.LastProduct();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

    }
}
