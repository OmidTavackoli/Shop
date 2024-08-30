using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Admin.Products
{
    public class EditProductCategoryViewModel : CreateProductCategoryViewModel
    {
        public long ProductCategoryId { get; set; }
        public string ImageName { get; set; }
    }
    public enum EditProductCategoryResult
    {
        IsExistUrlName,
        NotFound,
        Success
    }
}
