using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Domain.Models.Orders;
using Shop.Domain.ViewModels.Admin.Order;
using Shop.Domain.ViewModels.Paging;
using Shop.Presistance.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Presistance.Repositories
{
    public class OrderRepository(ShopDbContext context) : IOrderRepository
    {
        #region constractor
        private readonly ShopDbContext _context = context;

        #endregion


        #region orde

        public async Task<Order> CheckUserOrder(long userId)
        {
            return await _context.Orders.AsQueryable()
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsFinaly);
        }
        public async Task AddOrder(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetail> CheckOrderDetail(long orderId, long productId)
        {
            return await _context.OrderDetails.AsQueryable()
                .Where(c => c.OrderId == orderId && c.ProductId == productId && !c.IsDelete)
                .FirstOrDefaultAsync();
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }

        public async Task AddOrderDetail(OrderDetail order)
        {
            await _context.OrderDetails.AddAsync(order);
        }

        public async Task<Order> GetOrderById(long orderId)
        {
            return await _context.Orders.AsQueryable()
                .SingleOrDefaultAsync(c => c.Id == orderId);
        }

        public async Task<int> OrderSum(long orderId)
        {

            return await _context.OrderDetails.AsQueryable()
                .Where(c => c.OrderId == orderId && !c.IsDelete).SumAsync(c => c.Price * c.Count);
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
        }

        public async Task<Order> GetUserBasket(long orderId, long userId)
        {
            return await _context.Orders.AsQueryable()
                .Include(c => c.User)
                .Where(c => c.UserId == userId && c.Id == orderId)
                .Select(c => new Order
                {
                    UserId = c.UserId,
                    CreateDate = c.CreateDate,
                    Id = c.Id,
                    IsDelete = c.IsDelete,
                    IsFinaly = c.IsFinaly,
                    OrderSum = c.OrderSum,
                    OrderDetails = _context.OrderDetails.Where(c => !c.IsDelete && !c.Order.IsFinaly && c.Order.UserId == userId).Include(c => c.Product).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Order> GetOrderById(long orderId, long userId)
        {
            return await _context.Orders.AsQueryable()
               .SingleOrDefaultAsync(c => c.Id == orderId && c.UserId == userId);
        }

        public async Task<OrderDetail> GetOrderDetailById(long id)
        {
            return await _context.OrderDetails.AsQueryable()
                 .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Order> GetUserBasket(long userId)
        {
            return await _context.Orders.Include(c => c.OrderDetails).ThenInclude(c => c.Product).AsQueryable()
                .Where(c => c.UserId == userId && c.OrderState == OrderState.Processing && !c.IsFinaly && !c.IsDelete)
                .FirstOrDefaultAsync();
        }

        public async Task<ResultOrderStateViewModel> GetResultOrder()
        {
            //return await _context.Orders.AsQueryable()
            //    .Select(c=> new ResultOrderStateViewModel
            //    {
            //        CancelCount =
            //    })

            return new ResultOrderStateViewModel
            {
                CancelCount = await _context.Orders.AsQueryable().Where(c => c.OrderState == OrderState.Cancel && c.CreateDate.Day == DateTime.Now.Day).CountAsync(),
                RequestCount = await _context.Orders.AsQueryable().Where(c => c.OrderState == OrderState.Requested).CountAsync(),
                ProcessingCount = await _context.Orders.AsQueryable().Where(c => c.OrderState == OrderState.Processing && c.CreateDate.Day == DateTime.Now.Day).CountAsync(),
                SentCount = await _context.Orders.AsQueryable().Where(c => c.OrderState == OrderState.Sent).CountAsync()
            };
        }

        public async Task<FilterOrdersViewModel> FilterOrders(FilterOrdersViewModel filterOrders)
        {
            var query = _context.Orders.Include(c => c.OrderDetails).Include(c => c.User).AsQueryable();

            #region filter
            if (filterOrders.UserId.HasValue && filterOrders.UserId != 0)
            {
                query = query.Where(c => c.UserId == filterOrders.UserId);
            }
            #endregion

            #region state
            switch (filterOrders.OrderStateFilter)
            {
                case OrderStateFilter.All:
                    break;
                case OrderStateFilter.Requested:
                    query = query.Where(c => c.OrderState == OrderState.Requested);
                    break;
                case OrderStateFilter.Processing:
                    query = query.Where(c => c.OrderState == OrderState.Processing);

                    break;
                case OrderStateFilter.Sent:
                    query = query.Where(c => c.OrderState == OrderState.Sent);

                    break;
                case OrderStateFilter.Cancel:
                    query = query.Where(c => c.OrderState == OrderState.Cancel);

                    break;
            }
            #endregion


            var pager = Pager.Build(filterOrders.PageId, await query.CountAsync(), filterOrders.TakeEntity, filterOrders.CountForShowAfterAndBefor);
            var allData = await query.Paging(pager).ToListAsync();
            return filterOrders.SetPaging(pager).SetOrders(allData);
        }

        public async Task<Order> GetOrderDetail(long orderId)
        {
            return await _context.Orders.Include(c => c.OrderDetails).ThenInclude(c => c.Product).Include(c => c.User).AsQueryable()
                .Where(c => c.Id == orderId)
                .SingleOrDefaultAsync();
        }

        #endregion
    }
}
