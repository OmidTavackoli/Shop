using Shop.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.ViewModels.Account
{
    public class UserBasketViewModel
    {
        public int ItemCount { get; set; }
        public List<OrderDetail> Order { get; set; }
    }
}
