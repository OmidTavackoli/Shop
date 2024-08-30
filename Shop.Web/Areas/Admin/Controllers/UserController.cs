using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Account;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class UserController(IUserService userService) : AdminBaseController
    {
        #region constractor
        private readonly IUserService _userService = userService;
        #endregion

        #region filter user
        public async Task<IActionResult> FilterUser(FilterUserViewModel filter)
        {
            var data = await _userService.FilterUsers(filter);

            return View(data);
        }
        #endregion

        #region edit user
        [HttpGet]
        public async Task<IActionResult> EditUser(long userId) //id
        {
            var data = await _userService.GetEditUserFromAdmin(userId);

            if (data == null)
            {
                return NotFound();
            }

            ViewData["Roles"] = await _userService.GetAllActiveRoles();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserFromAdmin editUser)
        {
            ViewData["Roles"] = await _userService.GetAllActiveRoles();

            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserFromAdmin(editUser);

                switch (result)
                {
                    case EditUserFromAdminResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;
                    case EditUserFromAdminResult.Success:
                        TempData[SuccessMessage] = "ویراش کاربر با موفقیت انجام شد";
                        return RedirectToAction("FilterUser");
                }
            }

            return View(editUser);
        }

        #endregion


        #region filter roles
        public async Task<IActionResult> FilterRoles(FilterRolesViewModel filter)
        {
            return View(await _userService.FilterRoles(filter));
        }
        #endregion

        #region create Role
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(CreateOrEditRoleViewModel create)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateOrEditRole(create);

                switch (result)
                {
                    case CreateOrEditRoleResult.NotFound:
                        break;
                    case CreateOrEditRoleResult.NotExistPermissions:
                        TempData[WarningMessage] = "لطفا نقشی را انتخاب کنید";
                        break;
                    case CreateOrEditRoleResult.Success:
                        TempData[SuccessMessage] = "عملیات افزودن نقش با موفقیت انجام شد";
                        return RedirectToAction("FilterRoles");
                }
            }

            return View(create);
        }
        #endregion

        #region Edit Role
        [HttpGet]
        public async Task<IActionResult> EditRole(long roleId)
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();
            return View(await _userService.GetEditRoleById(roleId));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(CreateOrEditRoleViewModel create)
        {
            ViewData["Permissions"] = await _userService.GetAllActivePermission();
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateOrEditRole(create);

                switch (result)
                {
                    case CreateOrEditRoleResult.NotFound:
                        TempData[WarningMessage] = "نقشی با مشخصات وارد شده یافت نشد";
                        break;
                    case CreateOrEditRoleResult.NotExistPermissions:
                        TempData[WarningMessage] = "لطفا نقشی را انتخاب کنید";
                        break;
                    case CreateOrEditRoleResult.Success:
                        TempData[SuccessMessage] = "عملیات ویرایش نقش با موفقیت انجام شد";
                        return RedirectToAction("FilterRoles");
                }
            }

            return View(create);
        }
        #endregion
    }
}
