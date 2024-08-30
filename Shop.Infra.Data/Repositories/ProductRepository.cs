using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Paging;
using Shop.Domain.ViewModels.Site.Products;
using Shop.Presistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presistance.Repositories
{
    public class ProductRepository(ShopDbContext context) : IProductRepository
    {
        #region constractor
        private readonly ShopDbContext _context = context;



        #endregion

        #region product - admin

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        #region product-categories
        public async Task<bool> CheckUrlNameCategories(string urlName)
        {
            return await _context.ProductCategories.AsQueryable()
                .AnyAsync(c => c.UrlName == urlName);
        }

        public async Task AddProductCtaegory(ProductCategory productCategory)
        {
            await _context.ProductCategories.AddAsync(productCategory);
        }

        public async Task<ProductCategory> GetProductCategoryById(long id)
        {
            return await _context.ProductCategories.AsQueryable()
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CheckUrlNameCategories(string urlName, long CategoryId)
        {
            return await _context.ProductCategories.AsQueryable()
                .AnyAsync(c => c.UrlName == urlName && c.Id != CategoryId);
        }

        public void UpdateProductCtaegory(ProductCategory category)
        {
            _context.Update(category);
        }

        public async Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter)
        {
            var query = _context.ProductCategories.AsQueryable();

            #region filter
            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(c => EF.Functions.Like(c.Title, $"%{filter.Title}%"));
            }
            #endregion

            #region paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefor);
            var allData = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetPaging(pager).SetProductCategories(allData);
        }

        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            var query = _context.Products
                .Include(c => c.ProductSelectedCategories)
                .ThenInclude(c => c.ProductCategory)
                .AsQueryable();

            #region filter
            if (!string.IsNullOrEmpty(filter.ProductName))
            {
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{filter.ProductName}%"));
            }

            if (!string.IsNullOrEmpty(filter.FilterByCategory))
            {
                query = query.Where(c => c.ProductSelectedCategories.Any(s => s.ProductCategory.UrlName == filter.FilterByCategory));
            }

            #endregion

            #region state
            switch (filter.ProductState)
            {
                case ProductState.All:
                    query = query.Where(c => !c.IsDelete);
                    break;
                case ProductState.IsActice:
                    query = query.Where(c => c.IsActive);
                    break;
                case ProductState.Delete:
                    query = query.Where(c => c.IsDelete);
                    break;
            }

            switch (filter.ProductOrder)
            {
                case ProductOrder.All:
                    break;
                case ProductOrder.ProductNewss:
                    query = query.Where(c => c.IsActive).OrderByDescending(c => c.CreateDate);
                    break;
                case ProductOrder.ProductExp:
                    query = query.Where(c => c.IsActive).OrderByDescending(c => c.Price);
                    break;
                case ProductOrder.ProductInExprnsive:
                    query = query.Where(c => c.IsActive).OrderBy(c => c.Price);
                    break;
            }
            #endregion

            #region product box
            switch (filter.ProductBox)
            {
                case ProductBox.Default:
                    break;
                case ProductBox.ItemBoxInSite:

                    var pagerBox = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefor);
                    var allDataBox = await query.Paging(pagerBox).Select(c => new ProductItemViewModel
                    {
                        ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                        CommentCount = 0,
                        Price = c.Price,
                        ProductId = c.Id,
                        ProductImageName = c.ProductImageName,
                        ProductName = c.Name
                    }).ToListAsync();
                    return filter.SetPaging(pagerBox).SetProductsItem(allDataBox);
            }
            #endregion

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.CountForShowAfterAndBefor);
            var allData = await query.Paging(pager).ToListAsync();
            return filter.SetPaging(pager).SetProducts(allData);
        }

        public async Task AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task RemoveProductSelectedCategories(long productId)
        {
            var allProductSelectedCategories = await _context.ProductSelectedCategories.AsQueryable()
                .Where(c => c.ProductId == productId).ToListAsync();

            if (allProductSelectedCategories.Count > 0)
            {
                _context.ProductSelectedCategories.RemoveRange(allProductSelectedCategories);
            }

        }

        public async Task AddProductSelectedCategories(List<long> productSelectedCategories, long productId)
        {
            if (productSelectedCategories != null && productSelectedCategories.Count > 0)
            {
                var newProductSelectedCategories = new List<ProductSelectedCategories>();

                foreach (var categoryId in productSelectedCategories)
                {
                    newProductSelectedCategories.Add(new ProductSelectedCategories
                    {
                        ProductId = productId,
                        ProductCategoryId = categoryId
                    });
                }

                await _context.ProductSelectedCategories.AddRangeAsync(newProductSelectedCategories);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProductCategory>> GetAllProductCategories()
        {
            return await _context.ProductCategories.AsQueryable()
                 .Where(c => !c.IsDelete)
                 .ToListAsync();
        }

        public async Task<Product> GetProductById(long productId)
        {
            return await _context.Products.AsQueryable()
                .SingleOrDefaultAsync(c => c.Id == productId);
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
        }

        public async Task<List<long>> GetAllProductCategoriesId(long productId)
        {
            return await _context.ProductSelectedCategories.AsQueryable()
                .Where(c => c.ProductId == productId)
                .Select(c => c.ProductCategoryId)
                .ToListAsync();
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            var currentProduct = await _context.Products.AsQueryable()
                .Where(c => c.Id == productId).FirstOrDefaultAsync();

            if (currentProduct != null)
            {
                currentProduct.IsDelete = true;
                _context.Products.Update(currentProduct);
                await _context.SaveChangesAsync();


                return true;
            }

            return false;
        }

        public async Task<bool> RecoverProduct(long productId)
        {
            var currentProduct = await _context.Products.AsQueryable()
               .Where(c => c.Id == productId).FirstOrDefaultAsync();

            if (currentProduct != null)
            {
                currentProduct.IsDelete = false;
                _context.Products.Update(currentProduct);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task AddProductGalleries(List<ProductGalleries> productGalleries)
        {
            await _context.ProductGalleries.AddRangeAsync(productGalleries);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckProduct(long productId)
        {
            return await _context.Products.AsQueryable()
                .AnyAsync(c => c.Id == productId);
        }

        public async Task<List<ProductGalleries>> GetAllProductGalleries(long productId)
        {
            return await _context.ProductGalleries.AsQueryable()
                .Where(c => c.ProductId == productId && !c.IsDelete)
                .ToListAsync();
        }

        public async Task<ProductGalleries> GetProductGalleriesById(long id)
        {
            return await _context.ProductGalleries.AsQueryable()
                //.Where(c => c.Id == id)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task DeleteProductGallery(long id)
        {
            var currentGallery = await _context.ProductGalleries.AsQueryable()
                //.Where(c => c.Id == id)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (currentGallery != null)
            {
                currentGallery.IsDelete = true;

                _context.ProductGalleries.Update(currentGallery);
                await _context.SaveChangesAsync();
            }

        }

        public async Task AddProductFeatuers(ProductFeature feature)
        {
            await _context.ProductFeatures.AddAsync(feature);
        }

        public async Task<List<ProductFeature>> GetProductFeatures(long productId)
        {
            return await _context.ProductFeatures.AsQueryable()
                .Where(c => c.ProductId == productId && !c.IsDelete)
                .ToListAsync();
        }


        public async Task DeleteFeatuers(long id)
        {
            var currentFeatuer = await _context.ProductFeatures.AsQueryable().Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (currentFeatuer != null)
            {
                currentFeatuer.IsDelete = true;

                _context.ProductFeatures.Update(currentFeatuer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInSlider()
        {
            var allProduct = await _context.Products.Include(c => c.ProductComments).Include(c => c.ProductSelectedCategories).ThenInclude(c => c.ProductCategory).AsQueryable()
                .Select(c => new ProductItemViewModel
                {
                    ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                    CommentCount = c.ProductComments.Count(),
                    Price = c.Price,
                    ProductId = c.Id,
                    ProductImageName = c.ProductImageName,
                    ProductName = c.Name
                }).ToListAsync();

            return allProduct;
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName)
        {
            //var allProducts = await _context.ProductCategories.Include(c => c.ProductSelectedCategories).ThenInclude(c => c.Product)
            //     .Where(c => c.UrlName == hrefName)
            //     .Select(c => c.ProductSelectedCategories.Select(c => c.Product))
            //     .ToListAsync();


            var product = await _context.Products.Include(c => c.ProductComments).Include(c => c.ProductSelectedCategories).ThenInclude(c => c.ProductCategory).Where(c => c.ProductSelectedCategories.Any(c => c.ProductCategory.UrlName == hrefName)).ToListAsync();


            var data = product.Select(c => new ProductItemViewModel
            {
                ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                CommentCount = c.ProductComments.Count,
                Price = c.Price,
                ProductId = c.Id,
                ProductImageName = c.ProductImageName,
                ProductName = c.Name
            }).ToList();

            return data;
        }

        public async Task<List<ProductItemViewModel>> LastProduct()
        {
            var lastProducts = await _context.Products.Include(c => c.ProductSelectedCategories).ThenInclude(c => c.ProductCategory)
                .AsQueryable()
                .OrderByDescending(c => c.CreateDate)
                .Select(c => new ProductItemViewModel
                {
                    ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                    CommentCount = 0,
                    Price = c.Price,
                    ProductId = c.Id,
                    ProductImageName = c.ProductImageName,
                    ProductName = c.Name
                }).Take(8).ToListAsync();

            return lastProducts;
        }

        public async Task<ProductDetailViewModel> ShowProductDetail(long productId)
        {
            return await _context.Products.Include(c => c.ProductSelectedCategories).ThenInclude(c => c.ProductCategory).Include(c => c.ProductFeatures).Include(c => c.ProductGalleries).AsQueryable()
                .Include(c => c.ProductComments)
                .Where(c => c.Id == productId)
                .Select(c => new ProductDetailViewModel
                {
                    ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                    Description = c.Description,
                    Name = c.Name,
                    Price = c.Price,
                    ProductComment = c.ProductComments.Count(),
                    ProductFeatures = c.ProductFeatures.ToList(),
                    ProductId = c.Id,
                    ProductImageName = c.ProductImageName,
                    ShortDescription = c.ShortDescription,
                    ProductImages = c.ProductGalleries.Where(c => !c.IsDelete).Select(x => x.ImageName).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task AddProductComment(ProductComment comment)
        {
            await _context.ProductComments.AddAsync(comment);
        }

        public async Task<List<ProductComment>> AllProductCommentById(long productId)
        {
            return await _context.ProductComments.Include(c => c.User).AsQueryable()
                .Where(c => c.ProductId == productId)
                .ToListAsync();
        }

        public async Task<List<ProductItemViewModel>> GetRelatedProduct(string cateName, long productId)
        {
            var product = await _context.Products.Include(c => c.ProductComments).Include(c => c.ProductSelectedCategories).ThenInclude(c => c.ProductCategory).Where(c => c.ProductSelectedCategories.Any(c => c.ProductCategory.UrlName == cateName) && c.Id != productId).Take(6).ToListAsync();

            var data = product.Select(c => new ProductItemViewModel
            {
                ProductCategory = c.ProductSelectedCategories.Select(c => c.ProductCategory).First(),
                CommentCount = c.ProductComments.Count,
                Price = c.Price,
                ProductId = c.Id,
                ProductImageName = c.ProductImageName,
                ProductName = c.Name
            }).ToList();

            return data;
        }

        #endregion
        #endregion
    }
}
