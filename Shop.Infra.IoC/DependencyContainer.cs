using Microsoft.Extensions.DependencyInjection;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.Interfaces;
using Shop.Presistance.Repositories;

namespace Shop.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterService(IServiceCollection services)
        {
            #region services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISiteSettingService, SiteSettingService>();
            services.AddScoped<IOrderService, OrderService>();
            #endregion

            #region repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            #endregion

            #region tools
            services.AddScoped<IPasswordHelper, PasswordHelper>();
            services.AddScoped<ISmsService, SmsService>();
            #endregion
        }
    }
}
