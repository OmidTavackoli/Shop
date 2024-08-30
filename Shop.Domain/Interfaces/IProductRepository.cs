using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task SaveChanges();

        #region product categories
        Task<bool> CheckUrlNameCategories(string urlName);
        Task<bool> CheckUrlNameCategories(string urlName, long CategoryId);
        Task AddProductCtaegory(ProductCategory productCategory);
        Task<ProductCategory> GetProductCategoryById(long id);
        void UpdateProductCtaegory(ProductCategory category);
        Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter);
        Task<List<ProductCategory>> GetAllProductCategories();
        #endregion

        #region product
        Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter);
        Task AddProduct(Product product);
        Task RemoveProductSelectedCategories(long productId);
        Task<Product> GetProductById(long productId);
        Task AddProductSelectedCategories(List<long> productSelectedCategories, long productId);
        void UpdateProduct(Product product);
        Task<List<long>> GetAllProductCategoriesId(long productId);
        Task<bool> DeleteProduct(long productId);
        Task<bool> RecoverProduct(long productId);
        Task AddProductGalleries(List<ProductGalleries> productGalleries);
        Task<bool> CheckProduct(long productId);
        Task<List<ProductGalleries>> GetAllProductGalleries(long productId);
        Task<ProductGalleries> GetProductGalleriesById(long id);
        Task DeleteProductGallery(long id);
        Task AddProductFeatuers(ProductFeature feature);
        Task<List<ProductFeature>> GetProductFeatures(long productId);

        Task DeleteFeatuers(long id);

        Task<List<ProductItemViewModel>> ShowAllProductInSlider();

        Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName);
        Task<List<ProductItemViewModel>> LastProduct();

        Task<ProductDetailViewModel> ShowProductDetail(long productId);
        Task AddProductComment(ProductComment comment);
        Task<List<ProductComment>> AllProductCommentById(long productId);
        Task<List<ProductItemViewModel>> GetRelatedProduct(string cateName, long productId);
        #endregion
    }
}
