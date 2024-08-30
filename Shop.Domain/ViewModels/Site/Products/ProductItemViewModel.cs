using Shop.Domain.Models.ProductEntity;

namespace Shop.Domain.ViewModels.Site.Products
{
    public class ProductItemViewModel
    {
        #region properties
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageName { get; set; }
        public int Price { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public int CommentCount { get; set; }
        #endregion
    }
}
