using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using E_Commerce.Data.Status;
using E_Commerce.Infrustructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace E_Commerce.Infrustructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDBContext>>();
            try
            {
                await db.Database.MigrateAsync();

                foreach (var roleName in Role.Names.All)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new Role(roleName, $"{roleName} role");
                        await roleManager.CreateAsync(role);
                        logger.LogInformation("Created role: {Role}", roleName);
                    }
                }

                const string adminEmail = "arafaamr777@gmail.com";
                const string adminPwd = "Amr@123456";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = User.CreateAdmin(adminEmail, "System", "Admin");

                    // UserManager handles password hashing
                    var result = await userManager.CreateAsync(admin, adminPwd);

                    if (result.Succeeded)
                    {
                        // Assign Admin role via Identity
                        await userManager.AddToRoleAsync(admin, Role.Names.Admin);
                        logger.LogInformation(
                            "Seeded admin user: {Email} with role: Admin", adminEmail);
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogWarning("Failed to seed admin: {Errors}", errors);
                    }
                }

                if (!await db.categories.AnyAsync())
                {
                    var electronics = Category.Create("Electronics", "Electronic devices and components");
                    var clothing = Category.Create("Clothing & Apparel", "Fashion and garments");
                    var machinery = Category.Create("Machinery", "Industrial machinery");
                    var chemicals = Category.Create("Chemical Products", "Industrial chemicals");
                    var food = Category.Create("Food & Beverages", "Packaged food and drinks");
                    var rawMat = Category.Create("Raw Materials", "Industrial raw materials");

                    await db.categories.AddRangeAsync(
                        electronics, clothing, machinery, chemicals, food, rawMat);
                    await db.SaveChangesAsync();

                    var subs = new[]
                    {
                    Category.Create("Mobile Phones",          parentId: electronics.Id),
                    Category.Create("Computers & Laptops",    parentId: electronics.Id),
                    Category.Create("Electronic Components",  parentId: electronics.Id),
                    Category.Create("Audio & Video",          parentId: electronics.Id),
                    Category.Create("Men's Clothing",         parentId: clothing.Id),
                    Category.Create("Women's Clothing",       parentId: clothing.Id),
                    Category.Create("Sportswear",             parentId: clothing.Id),
                    Category.Create("Shoes & Footwear",       parentId: clothing.Id),
                    Category.Create("CNC Machines",           parentId: machinery.Id),
                    Category.Create("Packaging Machines",     parentId: machinery.Id),
                    Category.Create("Construction Equipment", parentId: machinery.Id),
                    Category.Create("Snacks & Confectionery", parentId: food.Id),
                    Category.Create("Beverages",              parentId: food.Id),
                    Category.Create("Frozen Foods",           parentId: food.Id),
                };

                    await db.categories.AddRangeAsync(subs);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} categories.", subs.Length + 6);
                }

                if (!await db.shippingRates.AnyAsync())
                {
                    var rates = new[]
                    {
                        ShippingRate.Create("Cairo", ShippingMethod.Standard, 45m, estimatedDays: 3),
                        ShippingRate.Create("Cairo", ShippingMethod.Express, 85m, estimatedDays: 1),
                        ShippingRate.Create("Cairo", ShippingMethod.Overnight, 150m, estimatedDays: 1),
                        ShippingRate.Create("Alexandria", ShippingMethod.Standard, 55m, estimatedDays: 4),
                        ShippingRate.Create("Alexandria", ShippingMethod.Express, 95m, estimatedDays: 2),
                        ShippingRate.Create("Giza", ShippingMethod.Standard, 45m, estimatedDays: 3),
                        ShippingRate.Create("Giza", ShippingMethod.Express, 85m, estimatedDays: 1),
                    };
                    await db.shippingRates.AddRangeAsync(rates);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} fallback shipping rates.", rates.Length);
                }

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed.");
                throw;
            }
        }
    }

}
