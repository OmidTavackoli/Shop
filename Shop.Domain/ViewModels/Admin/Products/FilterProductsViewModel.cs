using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Paging;
using Shop.Domain.ViewModels.Site.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class FilterProductsViewModel : BasePaging
    {
        public string ProductName { get; set; }
        public string FilterByCategory { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductItemViewModel> ProductItemViewModels { get; set; }
        public ProductState ProductState { get; set; }
        public ProductOrder ProductOrder { get; set; }
        public ProductBox ProductBox { get; set; }

        #region methods
        public FilterProductsViewModel SetProducts(List<Product> products)
        {
            this.Products = products;
            return this;
        }

        public FilterProductsViewModel SetProductsItem(List<ProductItemViewModel> productItemViews)
        {
            this.ProductItemViewModels = productItemViews;
            return this;
        }

        public FilterProductsViewModel SetPaging(BasePaging paging)
        {
            this.PageId = paging.PageId;
            this.AllEntityCount = paging.AllEntityCount;
            this.StartPage = paging.StartPage;
            this.EndPage = paging.EndPage;
            this.TakeEntity = paging.TakeEntity;
            this.CountForShowAfterAndBefor = paging.CountForShowAfterAndBefor;
            this.SkipEntitiy = paging.SkipEntitiy;
            this.PageCount = paging.PageCount;

            return this;
        }

        #endregion
    }
    public enum ProductState
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "فعال")]
        IsActice,
        [Display(Name = "حذف شده")]
        Delete
    }
    public enum ProductOrder
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "جدیدترین ها")]

        ProductNewss,
        [Display(Name = "گران ترین ها")]

        ProductExp,
        [Display(Name = "ارزان ترین ها")]

        ProductInExprnsive
    }
    public enum ProductBox
    {
        Default,
        ItemBoxInSite
    }
}
