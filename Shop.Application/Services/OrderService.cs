using Shop.Application.Interfaces;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Wallet;
using Shop.Domain.ViewModels.Account;
using Shop.Domain.ViewModels.Admin.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Services
{
    public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository, IWalletRepository walletRepository) : IOrderService
    {
        #region constractor
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IWalletRepository _walletRepository = walletRepository;

        #endregion

        #region order
        public async Task<long> AddOrder(long userId, long productId)
        {
            var product = await _productRepository.GetProductById(productId);

            var order = await _orderRepository.CheckUserOrder(userId);

            if (order == null)
            {
                // Add Order

                order = new Order
                {
                    UserId = userId,
                    IsFinaly = false,
                    OrderSum = product.Price,
                    OrderState = OrderState.Processing,
                    OrderDetails =
                    [
                        new OrderDetail()
                        {
                            ProductId =productId,
                            Price =product.Price,
                            Count =1
                        }
                    ]
                };

                await _orderRepository.AddOrder(order);
                await _orderRepository.SaveChanges();
            }
            else
            {
                var detail = await _orderRepository.CheckOrderDetail(order.Id, product.Id);
                if (detail != null)
                {
                    detail.Count += 1;
                    _orderRepository.UpdateOrderDetail(detail);
                    //await _orderRepository.SaveChanges();
                }
                else
                {
                    detail = new OrderDetail()
                    {
                        OrderId = order.Id,
                        Count = 1,
                        ProductId = productId,
                        Price = product.Price
                    };
                    await _orderRepository.AddOrderDetail(detail);
                    //await _orderRepository.SaveChanges();
                }

                await _orderRepository.SaveChanges();
            }

            await UpdatePriceOrder(order.Id);
            return order.Id;
        }

        public async Task ChangeIsFilnalyToOrder(long orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            order.IsFinaly = true;
            order.OrderState = OrderState.Requested;


            _orderRepository.UpdateOrder(order);
            await _orderRepository.SaveChanges();
        }

        public async Task<bool> ChangeStateToSent(long orderId)
        {
            var currentOrder = await _orderRepository.GetOrderById(orderId);

            if (currentOrder != null)
            {
                currentOrder.OrderState = OrderState.Sent;

                _orderRepository.UpdateOrder(currentOrder);
                await _orderRepository.SaveChanges();

                return true;
            }

            return false;
        }

        public async Task<FilterOrdersViewModel> FilterOrders(FilterOrdersViewModel filterOrders)
        {
            return await _orderRepository.FilterOrders(filterOrders);
        }

        public async Task<FinallyOrderResult> FinallyOrder(FinallyOrderViewModel finallyOrder, long userId)
        {
            if (userId != finallyOrder.UserId)
            {
                return FinallyOrderResult.HasNotUser;
            }


            var order = await _orderRepository.GetOrderById(finallyOrder.OrderId, userId);

            if (order == null || order.IsFinaly == true)
            {
                return FinallyOrderResult.NotFound;
            }

            if (await _walletRepository.GetUserWalletAmount(userId) >= order.OrderSum)
            {
                order.IsFinaly = true;
                order.OrderState = OrderState.Requested;


                var wallet = new UserWallet
                {
                    Amount = order.OrderSum,
                    Description = $"فاکتور شماره {order.Id}",
                    IsPay = true,
                    WalletType = WalletType.Bardasht,
                    UserId = userId
                };

                await _walletRepository.CreateWallet(wallet);

                _orderRepository.UpdateOrder(order);

                await _orderRepository.SaveChanges();

                return FinallyOrderResult.Suceess;
            }

            return FinallyOrderResult.Error;
        }

        public async Task<Order> GetOrderById(long orderId)
        {
            return await _orderRepository.GetOrderById(orderId);
        }

        public async Task<Order> GetOrderDetail(long orderId)
        {
            return await _orderRepository.GetOrderDetail(orderId);
        }

        public async Task<ResultOrderStateViewModel> GetResultOrder()
        {
            return await _orderRepository.GetResultOrder();
        }

        public async Task<Order> GetUserBasket(long orderId, long userId)
        {
            return await _orderRepository.GetUserBasket(orderId, userId);
        }

        public async Task<Order> GetUserBasket(long userId)
        {
            return await _orderRepository.GetUserBasket(userId);
        }

        public async Task<bool> RemoveOrderDetailFromOrder(long orderDetailId)
        {
            var orderDetail = await _orderRepository.GetOrderDetailById(orderDetailId);
            var order = await _orderRepository.GetOrderById(orderDetail.OrderId);


            if (orderDetail != null)
            {
                orderDetail.IsDelete = true;
                _orderRepository.UpdateOrderDetail(orderDetail);

                order.OrderSum = (await _orderRepository.OrderSum(order.Id) - orderDetail.Price);
                _orderRepository.UpdateOrder(order);

                await _orderRepository.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task UpdatePriceOrder(long orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);

            order.OrderSum = await _orderRepository.OrderSum(orderId);

            _orderRepository.UpdateOrder(order);
            await _orderRepository.SaveChanges();

        }
        #endregion
    }
}
