using Shop.Domain.Models.ProductEntity;
using Shop.Domain.ViewModels.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class FilterProductCategoriesViewModel : BasePaging
    {
        public string Title { get; set; }

        public List<ProductCategory> ProductCategories { get; set; }


        #region methods
        public FilterProductCategoriesViewModel SetProductCategories(List<ProductCategory> productCategories)
        {
            this.ProductCategories = productCategories;
            return this;
        }

        public FilterProductCategoriesViewModel SetPaging(BasePaging paging)
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
}
