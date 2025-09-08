using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using PluckFish.Attributes;
using PluckFish.Auth;
using PluckFish.Components;
using PluckFish.Components.Cache;
using PluckFish.Components.PostgresRepositories.API;
using PluckFish.Interfaces;
using PluckFish.Interfaces.API;
using PluckFish.Models;

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

                // JWT bearer token.
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "X-API-TOKEN",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
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
