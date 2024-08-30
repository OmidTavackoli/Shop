using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Order;
using Shop.Domain.ViewModels.Wallet;
using Shop.Web.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarinpalSandbox;

namespace Shop.Web.Areas.User.Controllers
{
    public class AccountController(IUserService userService, IWalletService walletService, IConfiguration configuration, IOrderService orderService) : UserBaseController
    {
        #region constractor
        private readonly IUserService _userService = userService;
        private readonly IWalletService _walletService = walletService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IOrderService _orderService = orderService;
        #endregion

        #region edit user profile
        [HttpGet("edit-user-profile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var user = await _userService.GetEditUserProfile(User.GetUserId());
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost("edit-user-profile"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfile, IFormFile userAvatar)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditProfile(User.GetUserId(), userAvatar, editUserProfile);
                switch (result)
                {
                    case EditUserProfileResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;
                    case EditUserProfileResult.Success:
                        TempData[SuccessMessage] = "عملیات ویرایش حساب کاربری با موفقیت انجام شد";
                        return RedirectToAction("EditUserProfile");
                }
            }
            return View(editUserProfile);
        }
        #endregion

        #region change password
        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangePassword(User.GetUserId(), changePassword);
                switch (result)
                {
                    case ChangePasswordResult.NotFound:
                        TempData[WarningMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;
                    case ChangePasswordResult.PasswordEqual:
                        TempData[InfoMessage] = "لطفا از کلمه عبور جدیدی استفاده کنید";
                        ModelState.AddModelError("NewPassword", "لطفا از کلمه عبور جدیدی استفاده کنید");
                        break;
                    case ChangePasswordResult.Success:
                        TempData[SuccessMessage] = "کلمه ی عبور شما با موفقیت تغیر یافت";
                        TempData[InfoMessage] = "لطفا جهت تکمیل فراید تغیر کلمه ی عبور ،مجددا وارد سایت شود";
                        await HttpContext.SignOutAsync();
                        return RedirectToAction("Login", "Account", new { area = "" });
                }
            }
            return View(changePassword);
        }
        #endregion

        #region charge wallet
        [HttpGet("charge-wallet")]
        public async Task<IActionResult> ChargeWallet()
        {
            //ToDo:Show Transaction List For User
            return View();
        }

        [HttpPost("charge-wallet"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ChargeWallet(ChargeWalletViewModel chargeWallet)
        {
            if (ModelState.IsValid)
            {
                var walletId = await _walletService.ChargeWallet(User.GetUserId(), chargeWallet, $"شارژ به مبلغ {chargeWallet.Amount}");

                #region payment
                var payment = new Payment(chargeWallet.Amount);
                var url = _configuration.GetSection("DefaultUrl")["Host"] + "/user/online-payment/" + walletId;
                var result = payment.PaymentRequest("شارژ کیف پول", url);

                if (result.Result.Status == 100)
                {
                    return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Result.Authority);
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در پرداخت به وجود آماده است،لطفا مجددا امتحان کنید";
                }

                #endregion
            }
            return View(chargeWallet);
        }
        #endregion

        #region online payment
        [HttpGet("online-payment/{id}")]
        public async Task<IActionResult> OnlinePayment(long id)
        {
            if (HttpContext.Request.Query["Status"] != "" && HttpContext.Request.Query["Status"].ToString().Equals("ok", StringComparison.CurrentCultureIgnoreCase) && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"]!;
                var wallet = await _walletService.GetUserWalletById(id);
                if (wallet != null)
                {
                    var payment = new Payment(wallet.Amount);
                    var result = payment.Verification(authority).Result;

                    if (result.Status == 100)
                    {
                        ViewBag.RefId = result.RefId;
                        ViewBag.Success = true;
                        await _walletService.UpdateWalletForCharge(wallet);
                    }
                    return View();
                }
                return NotFound();
            }
            return View();
        }
        #endregion

        #region user wallet
        [HttpGet("user-wallet")]
        public async Task<IActionResult> UserWallet(FilterWalletViewModel filter)
        {
            filter.UserId = User.GetUserId();
            return View(await _walletService.FilterWallets(filter));
        }
        #endregion

        #region user-basket
        [HttpGet("basket/{orderId}")]
        public async Task<IActionResult> UserBasket(long orderId)
        {
            var order = await _orderService.GetUserBasket(orderId, User.GetUserId());
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return View(order);
        }

        [HttpPost("basket/{orderId}"), ValidateAntiForgeryToken]
        public async Task<IActionResult> UserBasket(FinallyOrderViewModel finallyOrder)
        {

            if (finallyOrder.IsWallet)
            {
                var result = await _orderService.FinallyOrder(finallyOrder, User.GetUserId());

                switch (result)
                {
                    case FinallyOrderResult.HasNotUser:
                        TempData[ErrorMessage] = "سفارش شما یفت نشد";
                        break;
                    case FinallyOrderResult.NotFound:
                        TempData[ErrorMessage] = "سفارش شما یفت نشد";
                        break;
                    case FinallyOrderResult.Error:
                        TempData[ErrorMessage] = "موجودی کیف پول شما کافی نمیباشد";
                        return RedirectToAction("UserWallet");
                    case FinallyOrderResult.Suceess:
                        TempData[SuccessMessage] = "فاکتور شما با موفقیت پرداخت شد از خرید متشکریم";
                        return RedirectToAction("UserWallet");

                }
            }
            else
            {
                var order = await _orderService.GetOrderById(finallyOrder.OrderId);
                #region payment
                var payment = new Payment(order.OrderSum);
                var url = _configuration.GetSection("DefaultUrl")["Host"] + "/user/online-order/" + order.Id;
                var result = payment.PaymentRequest("شارژ کیف پول", url);

                if (result.Result.Status == 100)
                {
                    return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Result.Authority);
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در پرداخت به وجود آماده است،لطفا مجددا امتحان کنید";
                }

                #endregion
            }


            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return View();
        }

        #endregion

        #region delete-order-detail
        [HttpGet("delete-orderDetail/{orderDetailId}")]
        public async Task<IActionResult> DeleteOrderDetail(long orderDetailId)
        {
            var result = await _orderService.RemoveOrderDetailFromOrder(orderDetailId);

            if (result)
            {
                return JsonResponseStatus.Success();
            }
            return JsonResponseStatus.Error();
        }
        #endregion

        #region reload-price
        [HttpGet("reload-price")]
        public async Task<IActionResult> ReloadOrderPrice(long id)
        {
            var order = await _orderService.GetUserBasket(id, User.GetUserId());
            ViewBag.UserWalletAmount = await _walletService.GetUserWalletAmount(User.GetUserId());

            return PartialView("_OrderPrice", order);
        }
        #endregion

        #region order payment
        [HttpGet("online-order/{id}")]
        public async Task<IActionResult> OrderPayment(long id)
        {
            if (HttpContext.Request.Query["Status"] != "" && HttpContext.Request.Query["Status"].ToString().Equals("ok", StringComparison.CurrentCultureIgnoreCase) && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"]!;
                var order = await _orderService.GetOrderById(id);
                if (order != null)
                {
                    var payment = new Payment(order.OrderSum);
                    var result = payment.Verification(authority).Result;

                    if (result.Status == 100)
                    {
                        ViewBag.RefId = result.RefId;
                        ViewBag.Success = true;
                        await _orderService.ChangeIsFilnalyToOrder(order.Id);
                    }
                    return View();
                }
                return NotFound();
            }
            return View();
        }
        #endregion

        #region user-orders
        [HttpGet("user-orders")]
        public async Task<IActionResult> UserOrders(FilterOrdersViewModel filter)
        {
            filter.UserId = User.GetUserId();

            return View(await _orderService.FilterOrders(filter));
        }
        #endregion

        [HttpGet("add-favorite/{productId}")]
        public async Task<IActionResult> AddUserFavorite(long productId)
        {
            var result = await _userService.AddProductToFavoriet(productId, User.GetUserId());


            if (result)
                TempData[SuccessMessage] = "محصول موردنظر با موفقیت در قسمت علاقه مندی اضافه شد";
            else
                TempData[WarningMessage] = "محصول موردنظر قبلا در علاقه مندی ها اضافه شده است";


            return RedirectToAction("/");

        }

        [HttpGet("add-compare/{productId}")]
        public async Task<IActionResult> AddUserCompare(long productId)
        {
            var result = await _userService.AddProductToCompare(productId, User.GetUserId());


            if (result)
                TempData[SuccessMessage] = "محصول موردنظر با موفقیت در قسمت مقایسه اضافه شد";
            else
                TempData[WarningMessage] = "محصول موردنظر قبلا در مقایسه اضافه شده است";


            return RedirectToAction("/");

        }

        [HttpGet("removeAllUserCompare")]
        public async Task<IActionResult> RemoveAllUserCompares()
        {
            var result = await _userService.RemoveAllUserCompare(User.GetUserId());

            if (result)
                TempData[SuccessMessage] = "تمامی محصولاتی که در لیست مقایسه بود حذف شده";
            else
                TempData[WarningMessage] = "لیست مقایسه خالی می باشد";


            return Redirect("/");
        }
        [HttpGet("removeUserCompare/{productId}")]
        public async Task<IActionResult> RemoveUserCompare(long productId)
        {
            var result = await _userService.RemoveUserCompare(User.GetUserId(), productId);

            if (result)
                TempData[SuccessMessage] = "محصول مورد نظر از لیست مقایسه حذف شد";
            else
                TempData[WarningMessage] = "چنین محصولی در لیست مقایسه موجود نمی باشد";


            return Redirect("/");
        }
    }
}

