using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Interfaces;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Web.Extentions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Web.Areas.Admin.Controllers
{
    public class ProductController(IProductService productService) : AdminBaseController
    {
        #region constractor
        private readonly IProductService _productService = productService;
        #endregion

        #region product
        #region filter-products
        public async Task<IActionResult> FilterProducts(FilterProductsViewModel filter)
        {
            var data = await _productService.FilterProducts(filter);
            return View(data);
        }
        #endregion

        #region create-product
        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            TempData["Categories"] = await _productService.GetAllProductCategories();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductViewModel createProduct, IFormFile productImage)
        {
            //TempData["Categories"] = await _productService.GetAllProductCategories();
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProduct(createProduct, productImage);

                switch (result)
                {
                    case CreateProductResult.NotImage:
                        TempData[WarningMessage] = "لطفا برای محصول یک تصویر انتخاب کنید";
                        break;
                    case CreateProductResult.Success:
                        TempData[SuccessMessage] = "عملیات ثبت محصول با موفقیت انجام شد";
                        return RedirectToAction("FilterProducts");
                }
            }

            return View(createProduct);
        }
        #endregion

        #region edit-product
        [HttpGet]
        public async Task<IActionResult> EditProduct(long productId)
        {
            var data = await _productService.GetEditProduct(productId);
            if (data == null)
            {
                return NotFound();
            }
            TempData["Categories"] = await _productService.GetAllProductCategories();
            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(EditProductViewModel editProduct)
        {

            if (ModelState.IsValid)
            {
                var result = await _productService.EditProduct(editProduct);
                switch (result)
                {
                    case EditProductResult.NotFound:
                        TempData[WarningMessage] = "محصولی با مشخصات وارد شده یافت نشد";
                        break;
                    case EditProductResult.NotProductSelectedCategoryHasNull:
                        TempData[WarningMessage] = "لطفا دسته بندی محصول را وارد کنید";
                        break;
                    case EditProductResult.Success:
                        TempData[SuccessMessage] = ".ویرایش محصول با موفقیت انجام شد";
                        return RedirectToAction("FilterProducts");
                }
            }
            TempData["Categories"] = await _productService.GetAllProductCategories();

            return View(editProduct);
        }
        #endregion

        #region delete product
        public async Task<IActionResult> DeleteProduct(long productId)
        {
            var result = await _productService.DeleteProduct(productId);
            if (result)
            {
                TempData[SuccessMessage] = "محصول شما با موفقیت حذف شد";
                return RedirectToAction("FilterProducts");

            }

            TempData[WarningMessage] = "در حذف محصول خطایی رخ داده است";
            return RedirectToAction("FilterProducts");
        }
        #endregion

        #region recover producr
        public async Task<IActionResult> RecoverProduct(long productId)
        {
            var result = await _productService.RecoverProduct(productId);

            if (result)
            {
                TempData[SuccessMessage] = "محصول شما با موفقیت بازگردانی شد";
                return RedirectToAction("FilterProducts");

            }

            TempData[WarningMessage] = "در بازگردانی محصول خطایی رخ داده است";
            return RedirectToAction("FilterProducts");
        }
        #endregion

        #endregion

        #region category

        #region filter-product-categories

        public async Task<IActionResult> FilterProductCategories(FilterProductCategoriesViewModel filter)
        {
            return View(await _productService.FilterProductCategories(filter));
        }

        #endregion

        #region Create product category
        [HttpGet]
        public IActionResult CreateProductCategory()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductCategory(CreateProductCategoryViewModel createProductCategory, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductCategory(createProductCategory, image);
                switch (result)
                {
                    case CreateProductCategoryResult.IsExistUrlName:
                        TempData[WarningMessage] = "اسم Url تکراری است";
                        break;
                    case CreateProductCategoryResult.Success:
                        TempData[SuccessMessage] = "دسته بندی با موفقیت ثبت شد";
                        return RedirectToAction("FilterProductCategories");
                }
            }

            return View(createProductCategory);
        }
        #endregion

        #region edit product category
        [HttpGet]
        public async Task<IActionResult> EditProductCategory(long categoryId)
        {
            var data = await _productService.GetEditProductCategory(categoryId);

            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProductCategory(EditProductCategoryViewModel editProductCategory, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.EditProductCategory(editProductCategory, image);

                switch (result)
                {
                    case EditProductCategoryResult.IsExistUrlName:
                        TempData[WarningMessage] = "اسم Url تکراری است";
                        break;
                    case EditProductCategoryResult.NotFound:
                        TempData[ErrorMessage] = "دسته بندی با مشخصات وارد شده یافت نشد";
                        break;
                    case EditProductCategoryResult.Success:
                        TempData[SuccessMessage] = "دسته بندی با موفقیت ویرایش شد";
                        return RedirectToAction("FilterProductCategories");
                }
            }

            return View(editProductCategory);
        }
        #endregion
        #endregion

        #region product galleries
        #region create
        public IActionResult GalleryProduct(long productId)
        {
            ViewBag.productId = productId;
            return View();
        }


        public async Task<IActionResult> AddImageToProduct(List<IFormFile> images, long productId)
        {
            var result = await _productService.AddProductGallery(productId, images);
            if (result)
            {
                return JsonResponseStatus.Success();
            }
            return JsonResponseStatus.Error();
        }
        #endregion

        #region list product galleries
        public async Task<IActionResult> ProductGalleries(long productId)
        {
            var data = await _productService.GetAllProductGalleries(productId);

            return View(data);
        }
        #endregion

        #region delete image
        public async Task<IActionResult> DeleteImage(long galleryId)
        {
            await _productService.DeleteImage(galleryId);
            return RedirectToAction("FilterProducts");
        }
        #endregion

        #endregion

        #region product featuers
        #region create
        [HttpGet]
        public IActionResult CreateProductFeatuers(long productId)
        {
            var model = new CreateProductFeatuersViewModel
            {
                ProductId = productId
            };

            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProductFeatuers(CreateProductFeatuersViewModel featuersViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProductFetuers(featuersViewModel);

                switch (result)
                {
                    case CreateProductFeatuersResult.Error:
                        TempData[ErrorMessage] = "در ثبت ویژگی خطایی رخ داده است";
                        break;
                    case CreateProductFeatuersResult.Success:
                        TempData[SuccessMessage] = "ویژگی با موفقیت ثبت شد";
                        return RedirectToAction("FilterProducts");
                }
            }

            return View(featuersViewModel);
        }
        #endregion

        #region product featuers
        public async Task<IActionResult> ProductFeatuers(long productId)
        {
            return View(await _productService.GetProductFeatures(productId));
        }
        #endregion

        #region delete productFeatuer
        public async Task<IActionResult> DeleteFeatuers(long featuerId)
        {
            await _productService.DeleteFeatuers(featuerId);

            return RedirectToAction("FilterProducts");
        }
        #endregion

        #endregion
    }
}
