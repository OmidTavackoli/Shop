using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<long> AddOrder(long userId, long productId);

        Task UpdatePriceOrder(long orderId);
        Task<Order> GetUserBasket(long orderId, long userId);
        Task<Order> GetUserBasket(long userId);
        Task<FinallyOrderResult> FinallyOrder(FinallyOrderViewModel finallyOrder, long userId);
        Task<Order> GetOrderById(long orderId);

        Task<bool> RemoveOrderDetailFromOrder(long orderDetailId);

        Task ChangeIsFilnalyToOrder(long orderId);
        Task<ResultOrderStateViewModel> GetResultOrder();
        Task<FilterOrdersViewModel> FilterOrders(FilterOrdersViewModel filterOrders);

        Task<bool> ChangeStateToSent(long orderId);

        Task<Order> GetOrderDetail(long orderId);
    }
}
