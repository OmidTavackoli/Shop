using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Sliders;
using Shop.Web.Extentions;
using System.Threading.Tasks;

namespace Shop.Web.ViewComponets
{
    #region site header
    public class SiteHeaderViewComponent(IUserService userService, IOrderService orderService) : ViewComponent
    {
        private readonly IUserService _userService = userService;
        private readonly IOrderService _orderService = orderService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                ViewBag.User = await _userService.GetUserByPhoneNumber(User.Identity.Name);
                ViewBag.Order = await _orderService.GetUserBasket(User.GetUserId());
                ViewBag.UserCompare = await _userService.GetUserCompares(User.GetUserId());
                ViewBag.FavoriteCount = await _userService.UserFavoriteCount(User.GetUserId());
            }
            return View("SiteHeader");
        }
    }
    #endregion

    #region site footer
    public class SiteFooterViewComponent : ViewComponent
    {
        public IViewComponentResult InvokeAsync()
        {
            return View("SiteFooter");
        }
    }
    #endregion

    #region slider - home
    public class SliderHomeViewComponent(ISiteSettingService siteSettingService) : ViewComponent
    {
        #region constractor
        private readonly ISiteSettingService _siteSettingService = siteSettingService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterSlidersViewModel = new FilterSlidersViewModel()
            {
                TakeEntity = 10
            };

            var data = await _siteSettingService.FilterSliders(filterSlidersViewModel);

            return View("SliderHome", data);
        }
    }
    #endregion

    #region popular category - home
    public class PopularCategoryViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterCategory = new FilterProductCategoriesViewModel()
            {
                TakeEntity = 6
            };

            var data = await _productService.FilterProductCategories(filterCategory);

            return View("PopularCategory", data);
        }
    }
    #endregion

    #region popular category - home
    public class SideBarCategoryViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterCategory = new FilterProductCategoriesViewModel();

            var data = await _productService.FilterProductCategories(filterCategory);

            return View("SideBarCategory", data);
        }
    }
    #endregion

    #region All-productInSlider - home
    public class AllProductInSliderViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var data = await _productService.ShowAllProductInSlider();

            return View("AllProductInSlider", data);
        }
    }
    #endregion

    #region All-productInCategoryPc - home
    public class AllInCategoryPcViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var data = await _productService.ShowAllProductInCategory("pc");

            return View("AllInCategoryPc", data);
        }
    }
    #endregion

    #region All-productInCategoryPc - home
    public class AllInCategoryTvViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var data = await _productService.ShowAllProductInCategory("tv");

            return View("AllInCategoryTv", data);
        }
    }
    #endregion

    #region All-productInCategoryPc - home
    public class ProductCommentsViewComponent(IProductService productService) : ViewComponent
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        public async Task<IViewComponentResult> InvokeAsync(long productId)
        {

            var data = await _productService.AllProductCommentById(productId);

            return View("ProductComments", data);
        }
    }
    #endregion
}
