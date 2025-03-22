using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoopingWebApplication.Models;

namespace ShoopingWebApplication.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any() && !context.Carts.Any() && !context.Orders.Any())
            {
                await SeedData(context, userManager);
            }
        }

        private static async Task SeedData(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            if (!context.Categories.Any() && !context.Products.Any())
            {
                // Seed Products
                var categories = new List<Category>
                {
                    new Category { Name = "AJ1", ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/083/401/929/original/14741_01.jpg.jpeg?action=crop&width=400" },
                    new Category { Name = "Dunks", ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/093/184/226/original/1173790_01.jpg.jpeg?action=crop&width=400" },
                    new Category { Name = "Air force1", ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/090/524/108/original/1174801_01.jpg.jpeg?action=crop&width=800" }

                };

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();


                int AJ1CategoryId = categories.FirstOrDefault(c => c.Name == "AJ1").CategoryId;
                int DunksCategoryId = categories.FirstOrDefault(c => c.Name == "Dunks").CategoryId;
                int Airforce1CategoryId = categories.FirstOrDefault(c => c.Name == "Air force1").CategoryId;

                var products = new List<Product>
                {
                    new Product { Name = "Chicago AJ1", Description = "Air Jordan 1 Retro High OG 'Chicago' 2015 ", Price = 300.00M, StockQuantity = 100, ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/083/401/929/original/14741_01.jpg.jpeg?action=crop&width=400", CategoryId = AJ1CategoryId },
                    new Product { Name = "Air Jordan 1 'Olive'", Description = "Travis Scott x Wmns Air Jordan 1 Retro Low OG 'Olive'", Price = 1000.00M, StockQuantity = 50, ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/086/872/318/original/1049526_01.jpg.jpeg?action=crop&width=800", CategoryId =  AJ1CategoryId},
                    new Product { Name = "Air Jordan 1 'Royal Toe'", Description = "Air Jordan 1 Retro High OG 'Royal Toe'", Price = 250.00M, StockQuantity = 75, ImageUrl = "https://image.goat.com/transform/v1/attachments/product_template_additional_pictures/images/081/096/195/original/519961_01.jpg.jpeg?action=crop&width=400", CategoryId = AJ1CategoryId }

                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            var adminEmail = "admin@example.com";
            var adminPassword = "AdminPassword123!";

            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            if (userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
