using Shop.Domain.ViewModels.Paging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.ViewModels.Admin.Order
{
    public class FilterOrdersViewModel : BasePaging
    {
        public long? UserId { get; set; }
        public OrderStateFilter OrderStateFilter { get; set; }
        public List<Models.Orders.Order> Orders { get; set; }


        #region methods
        public FilterOrdersViewModel SetOrders(List<Models.Orders.Order> orders)
        {
            this.Orders = orders;
            return this;
        }

        public FilterOrdersViewModel SetPaging(BasePaging paging)
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

    public enum OrderStateFilter
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "درخواست شده")]
        Requested,
        [Display(Name = "در حال بررسی")]
        Processing,
        [Display(Name = "ارسال شده")]
        Sent,
        [Display(Name = "لغو شده")]
        Cancel
    }
}
