using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Web.Extentions;
using System.Threading.Tasks;

namespace Shop.Web.Areas.User.ViewComponents
{
    public class UserSideBarViewComponent(IUserService userService) : ViewComponent
    {
        #region constrator
        private readonly IUserService _userService = userService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var user = await _userService.GetUserById(User.GetUserId());
                return View("UserSideBar", user);
            }

            return View("UserSideBar");
        }
        #endregion
    }

    public class UserInformationViewComponent(IUserService userService) : ViewComponent
    {
        #region constrator
        private readonly IUserService _userService = userService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var user = await _userService.GetUserById(User.GetUserId());
                return View("UserInformation", user);
            }

            return View("UserInformation");
        }
        #endregion
    }
}
