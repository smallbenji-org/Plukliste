using PluckFish.Components;
using PluckFish.Interfaces;

namespace PluckFish
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("defaultConnection")))
            {
                builder.Services.AddTransient<IProductRepository, PostgresProductRepository>();
                builder.Services.AddTransient<IPickingListRepository, PostgresPickingListRepository>();
                builder.Services.AddTransient<PostgresEnsureTables>();

                builder.Services.AddTransient<PostGres>();
            }
            else
            {
                builder.Services.AddSingleton<IProductRepository, DummyProductRepository>();
                builder.Services.AddSingleton<IPickingListRepository, DummyPickinglistRepository>();
            }


            WebApplication app = builder.Build();

            app.Services.GetRequiredService<PostgresEnsureTables>().Ensure();

            //app.Services.GetRequiredService<IPickingListRepository>().AddPickingList(new Models.PickingList
            //{
            //    Name = "Name",
            //    Adresse = "Adresse",
            //    Forsendelse = "GLS",
            //    Lines = new List<Models.Item>
            //    {
            //        new Models.Item
            //        {
            //            Amount = 1,
            //            Product = new Models.Product
            //            {
            //                Name = "Spand",
            //                ProductID = "test-spand"
            //            },
            //            Type = Models.ItemType.Fysisk
            //        }
            //    } 
            //});

            //app.Services.GetRequiredService<IPickingListRepository>().AddProductToPickingList(new Models.PickingList
            //{
            //    Id = 1
            //}, new Models.Item
            //{
            //    Product = new Models.Product { ProductID = "test-spand"},
            //    Amount = 2,
            //    Type = Models.ItemType.Fysisk
            //});
            app.Services.GetRequiredService<IPickingListRepository>().GetAllPickingList();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
