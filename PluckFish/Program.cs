using PluckFish.Components;
using PluckFish.Interfaces;
using Microsoft.AspNetCore.Identity;
using PluckFish.Models;
using PluckFish.Auth;
using Microsoft.Extensions.Caching.Memory;
using PluckFish.Components.Cache;
using PluckFish.Interfaces.API;
using PluckFish.Components.PostgresRepositories.API;
using Microsoft.AspNetCore.Mvc;
using PluckFish.Attributes;

namespace PluckFish
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
            });

            builder.Services.AddMemoryCache();

            if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("defaultConnection")))
            {
                builder.Services.AddTransient<IAuthRepository, PostgresAuthRepository>();
                builder.Services.AddTransient<PostgresEnsureTables>();

                builder.Services.AddScoped<IPickingListRepository, PostgresPickingListRepository>();
                builder.Services.Decorate<IPickingListRepository, CachedPickingListRepository>();

                builder.Services.AddScoped<IProductRepository, PostgresProductRepository>();
                builder.Services.Decorate<IProductRepository, CachedProductRepository>();

                builder.Services.AddScoped<IStockRepository, PostgresStockRepository>();
                builder.Services.Decorate<IStockRepository, CachedStockRepository>();

                builder.Services.AddScoped<IVerificationApi, PostgresApiTokenRepository>();

                builder.Services.AddTransient<PostGres>();
            }
            else
            {
                builder.Services.AddSingleton<IProductRepository, DummyProductRepository>();
                builder.Services.AddSingleton<IPickingListRepository, DummyPickinglistRepository>();
            }

            builder.Services.AddTransient<StockHelper>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, ApiDesc) =>
                {
                    if (ApiDesc.ActionDescriptor.EndpointMetadata.Any(em => em is HideFromSwaggerAttribute))
                        return false;

                    return ApiDesc.ActionDescriptor.EndpointMetadata.Any(m => m is ApiControllerAttribute && !(m is HideFromSwaggerAttribute));
                });
            });

            WebApplication app = builder.Build();

            app.Services.GetRequiredService<PostgresEnsureTables>().Ensure();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            app.Run();
        }
    }
}
