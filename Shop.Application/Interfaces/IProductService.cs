using Microsoft.AspNetCore.Http;
using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface IProductService
    {
        #region product-admin
        Task<CreateProductCategoryResult> CreateProductCategory(CreateProductCategoryViewModel createCategory, IFormFile image);
        Task<EditProductCategoryViewModel> GetEditProductCategory(long categoryId);
        Task<EditProductCategoryResult> EditProductCategory(EditProductCategoryViewModel editProductCategory, IFormFile image);
        Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter);
        Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter);
        Task<CreateProductResult> CreateProduct(CreateProductViewModel createProduct, IFormFile imageProduct);
        Task<List<ProductCategory>> GetAllProductCategories();

        Task<EditProductViewModel> GetEditProduct(long productId);
        Task<EditProductResult> EditProduct(EditProductViewModel editProduct);
        Task<bool> DeleteProduct(long productId);
        Task<bool> RecoverProduct(long productId);
        Task<bool> AddProductGallery(long productId, List<IFormFile> images);

        Task<List<ProductGalleries>> GetAllProductGalleries(long productId);
        Task DeleteImage(long galleryId);
        Task<CreateProductFeatuersResult> CreateProductFetuers(CreateProductFeatuersViewModel createProductFeatuers);
        Task<List<ProductFeature>> GetProductFeatures(long productId);
        Task DeleteFeatuers(long id);

        Task<List<ProductItemViewModel>> ShowAllProductInSlider();

        Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName);
        Task<List<ProductItemViewModel>> LastProduct();
        Task<List<ProductItemViewModel>> GetRelatedProduct(string cateName, long productId);
        Task<ProductDetailViewModel> ShowProductDetail(long productId);
        Task<CreateProductCommentResult> CreateProductComment(CreateProductCommentViewModel createProduct, long userId);
        Task<List<ProductComment>> AllProductCommentById(long productId);
        #endregion
    }
}
