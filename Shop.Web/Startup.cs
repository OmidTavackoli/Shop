using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Shop.Infra.IoC;
using GoogleReCaptcha.V3.Interface;
using GoogleReCaptcha.V3;
using Shop.Presistance.Context;

namespace Shop.Web
{
    public class Startup(IConfiguration configuration)
    {
        #region Constarctor
        public IConfiguration Configuration { get; } = configuration;
        #endregion

        #region Services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            #region context
            services.AddDbContext<ShopDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"));
            });
            #endregion

            #region authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/log-Out";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(43200);

            });
            #endregion

            #region IoC
            RegisterService(services);
            services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: [UnicodeRanges.BasicLatin, UnicodeRanges.Arabic]));

            services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();
            #endregion

        }

        #endregion

        #region Configure
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseStatusCodePagesWithReExecute("/Home/Error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #endregion

        #region IoC
        public static void RegisterService(IServiceCollection services)
        {
            DependencyContainer.RegisterService(services);
        }
        #endregion
    }
}
