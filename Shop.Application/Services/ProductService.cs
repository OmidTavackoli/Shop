using Microsoft.AspNetCore.Http;
using Shop.Application.Extensions;
using Shop.Application.Interfaces;
using Shop.Application.Utils;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Admin.Products;
using Shop.Domain.ViewModels.Site.Products;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class ProductService(IProductRepository productRepository, IUserRepository userRepository) : IProductService
    {
        #region constarctor
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IUserRepository _userRepository = userRepository;


        #endregion

        #region product - admin
        #region product categories
        public async Task<CreateProductCategoryResult> CreateProductCategory(CreateProductCategoryViewModel createCategory, IFormFile image)
        {
            if (await _productRepository.CheckUrlNameCategories(createCategory.UrlName)) return CreateProductCategoryResult.IsExistUrlName;

            var newCategory = new ProductCategory
            {
                UrlName = createCategory.UrlName,
                Title = createCategory.Title,
                ParentId = null,
                IsDelete = false
            };
            if (image != null && image.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
                image.AddImageToServer(imageName, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer);

                newCategory.ImageName = imageName;
            }

            await _productRepository.AddProductCtaegory(newCategory);
            await _productRepository.SaveChanges();

            return CreateProductCategoryResult.Success;
        }

        public async Task<EditProductCategoryResult> EditProductCategory(EditProductCategoryViewModel editProductCategory, IFormFile image)
        {
            var productcategory = await _productRepository.GetProductCategoryById(editProductCategory.ProductCategoryId);

            if (productcategory == null) return EditProductCategoryResult.NotFound;

            if (await _productRepository.CheckUrlNameCategories(editProductCategory.UrlName, editProductCategory.ProductCategoryId)) return EditProductCategoryResult.IsExistUrlName;

            productcategory.UrlName = editProductCategory.UrlName;
            productcategory.Title = editProductCategory.Title;

            if (image != null && image.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
                image.AddImageToServer(imageName, PathExtensions.CategoryOrginServer, 150, 150, PathExtensions.CategoryThumbServer, productcategory.ImageName);

                productcategory.ImageName = imageName;
            }

            _productRepository.UpdateProductCtaegory(productcategory);

            await _productRepository.SaveChanges();

            return EditProductCategoryResult.Success;
        }

        public async Task<FilterProductCategoriesViewModel> FilterProductCategories(FilterProductCategoriesViewModel filter)
        {
            return await _productRepository.FilterProductCategories(filter);
        }


        public async Task<EditProductCategoryViewModel> GetEditProductCategory(long categoryId)
        {
            var productcategory = await _productRepository.GetProductCategoryById(categoryId);

            if (productcategory != null)
            {
                return new EditProductCategoryViewModel
                {
                    ImageName = productcategory.ImageName,
                    ProductCategoryId = productcategory.Id,
                    Title = productcategory.Title,
                    UrlName = productcategory.UrlName
                };
            }

            return null;

        }
        #endregion

        #region product

        public async Task<FilterProductsViewModel> FilterProducts(FilterProductsViewModel filter)
        {
            return await _productRepository.FilterProducts(filter);
        }

        public async Task<CreateProductResult> CreateProduct(CreateProductViewModel createProduct, IFormFile imageProduct)
        {
            #region product
            var newProduct = new Product
            {
                Name = createProduct.Name,
                Price = createProduct.Price,
                Description = createProduct.Description,
                ShortDescription = createProduct.ShortDescription,
                IsActive = createProduct.IsActive
            };


            if (imageProduct != null && imageProduct.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(imageProduct.FileName);

                imageProduct.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 215, 215, PathExtensions.ProductThumbServer);

                newProduct.ProductImageName = imageName;
            }
            else
            {
                return CreateProductResult.NotImage;
            }


            await _productRepository.AddProduct(newProduct);
            await _productRepository.SaveChanges();
            await _productRepository.AddProductSelectedCategories(createProduct.ProductSelectedCategory, newProduct.Id);

            return CreateProductResult.Success;

            #endregion
        }

        public async Task<List<ProductCategory>> GetAllProductCategories()
        {
            return await _productRepository.GetAllProductCategories();
        }

        public async Task<EditProductViewModel> GetEditProduct(long productId)
        {
            var currentProduct = await _productRepository.GetProductById(productId);

            if (currentProduct != null)
            {
                return new EditProductViewModel
                {
                    Description = currentProduct.Description,
                    IsActive = currentProduct.IsActive,
                    Name = currentProduct.Name,
                    Price = currentProduct.Price,
                    ShortDescription = currentProduct.ShortDescription,
                    ProductImageName = currentProduct.ProductImageName,
                    ProductSelectedCategory = await _productRepository.GetAllProductCategoriesId(productId)
                };
            }

            return null;
        }

        public async Task<EditProductResult> EditProduct(EditProductViewModel editProduct)
        {
            var product = await _productRepository.GetProductById(editProduct.ProductId);
            if (product == null) return EditProductResult.NotFound;
            if (editProduct.ProductSelectedCategory == null) return EditProductResult.NotProductSelectedCategoryHasNull;

            #region edit product
            product.ShortDescription = editProduct.ShortDescription;
            product.Description = editProduct.Description;
            product.IsActive = editProduct.IsActive;
            product.Price = editProduct.Price;
            product.Name = editProduct.Name;
            if (editProduct.ProductImage != null && editProduct.ProductImage.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(editProduct.ProductImage.FileName);

                editProduct.ProductImage.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer, product.ProductImageName);

                product.ProductImageName = imageName;
            }
            #endregion
            _productRepository.UpdateProduct(product);

            await _productRepository.RemoveProductSelectedCategories(editProduct.ProductId);
            await _productRepository.AddProductSelectedCategories(editProduct.ProductSelectedCategory, editProduct.ProductId);


            await _productRepository.SaveChanges();

            return EditProductResult.Success;
        }

        public async Task<bool> DeleteProduct(long productId)
        {
            return await _productRepository.DeleteProduct(productId);
        }

        public async Task<bool> RecoverProduct(long productId)
        {
            return await _productRepository.RecoverProduct(productId);
        }

        public async Task<bool> AddProductGallery(long productId, List<IFormFile> images)
        {

            if (!await _productRepository.CheckProduct(productId))
            {
                return false;
            }

            if (images != null && images.Count > 0)
            {
                var productGallery = new List<ProductGalleries>();
                foreach (var image in images)
                {
                    if (image.IsImage())
                    {
                        var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(image.FileName);
                        image.AddImageToServer(imageName, PathExtensions.ProductOrginServer, 255, 273, PathExtensions.ProductThumbServer);


                        productGallery.Add(new ProductGalleries
                        {
                            ImageName = imageName,
                            ProductId = productId
                        });
                    }
                }

                await _productRepository.AddProductGalleries(productGallery);

            }
            return true;
        }

        public async Task<List<ProductGalleries>> GetAllProductGalleries(long productId)
        {
            return await _productRepository.GetAllProductGalleries(productId);
        }

        public async Task DeleteImage(long galleryId)
        {
            var productGallery = await _productRepository.GetProductGalleriesById(galleryId);
            if (productGallery != null)
            {
                UploadImageExtension.DeleteImage(productGallery.ImageName, PathExtensions.ProductOrginServer, PathExtensions.ProductThumbServer);
                await _productRepository.DeleteProductGallery(galleryId);
            }
        }

        public async Task<CreateProductFeatuersResult> CreateProductFetuers(CreateProductFeatuersViewModel createProductFeatuers)
        {
            if (!await _productRepository.CheckProduct(createProductFeatuers.ProductId))
            {
                return CreateProductFeatuersResult.Error;
            }

            var newProductFeaturs = new ProductFeature
            {
                FeatuerTitle = createProductFeatuers.Title,
                FeatureValue = createProductFeatuers.Value,
                ProductId = createProductFeatuers.ProductId
            };

            await _productRepository.AddProductFeatuers(newProductFeaturs);
            await _productRepository.SaveChanges();
            return CreateProductFeatuersResult.Success;
        }

        public async Task<List<ProductFeature>> GetProductFeatures(long productId)
        {
            return await _productRepository.GetProductFeatures(productId);
        }

        public async Task DeleteFeatuers(long id)
        {
            await _productRepository.DeleteFeatuers(id);
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInSlider()
        {
            return await _productRepository.ShowAllProductInSlider();
        }

        public async Task<List<ProductItemViewModel>> ShowAllProductInCategory(string hrefName)
        {
            return await _productRepository.ShowAllProductInCategory(hrefName);
        }

        public async Task<List<ProductItemViewModel>> LastProduct()
        {
            return await _productRepository.LastProduct();
        }

        public async Task<ProductDetailViewModel> ShowProductDetail(long productId)
        {
            return await _productRepository.ShowProductDetail(productId);
        }

        public async Task<CreateProductCommentResult> CreateProductComment(CreateProductCommentViewModel createProduct, long userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return CreateProductCommentResult.CheckUser;
            }

            if (!await _productRepository.CheckProduct(createProduct.ProductId))
            {
                return CreateProductCommentResult.CheckProduct;
            }

            var newComment = new ProductComment
            {
                ProductId = createProduct.ProductId,
                UserId = userId,
                Text = createProduct.Text
            };
            await _productRepository.AddProductComment(newComment);
            await _productRepository.SaveChanges();

            return CreateProductCommentResult.Suucess;
        }

        public async Task<List<ProductComment>> AllProductCommentById(long productId)
        {
            return await _productRepository.AllProductCommentById(productId);
        }

        public async Task<List<ProductItemViewModel>> GetRelatedProduct(string cateName, long productId)
        {
            return await _productRepository.GetRelatedProduct(cateName, productId);
        }
        #endregion
        #endregion
    }
}
