using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using mvc_project.Models;

namespace mvc_project.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Elektronik", Description = "Telefon, bilgisayar, aksesuar ve daha fazlası", ImageUrl = "https://cdn-icons-png.flaticon.com/512/1041/1041916.png" },
                    new Category { Name = "Moda", Description = "Giyim, ayakkabı, çanta ve aksesuarlar", ImageUrl = "https://cdn-icons-png.flaticon.com/512/892/892458.png" },
                    new Category { Name = "Ev & Yaşam", Description = "Mobilya, dekorasyon, mutfak ürünleri", ImageUrl = "https://cdn-icons-png.flaticon.com/512/1946/1946436.png" },
                    new Category { Name = "Kozmetik", Description = "Makyaj, cilt bakımı, parfüm", ImageUrl = "https://cdn-icons-png.flaticon.com/512/2921/2921822.png" },
                    new Category { Name = "Spor & Outdoor", Description = "Spor giyim, ekipman, outdoor ürünler", ImageUrl = "https://cdn-icons-png.flaticon.com/512/1041/1041913.png" }
                );
                await context.SaveChangesAsync();
            }

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Roller
            if (!await roleManager.RoleExistsAsync("Supplier"))
                await roleManager.CreateAsync(new IdentityRole("Supplier"));

            if (!await roleManager.RoleExistsAsync("WarehouseManager"))
                await roleManager.CreateAsync(new IdentityRole("WarehouseManager"));

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Supplier Kullanıcı
            var supplierEmail = "supplier@example.com";
            var supplier2 = context.Users.FirstOrDefault(u => u.UserName == "supplier2@example.com");
            var supplierUser = await userManager.FindByEmailAsync(supplierEmail);
            if (supplierUser == null)
            {
                supplierUser = new AppUser
                {
                    UserName = supplierEmail,
                    Email = supplierEmail,
                    FullName = "Varsayılan Tedarikçi",
                    Address = "Tedarikçi Adresi",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(supplierUser, "Supplier123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(supplierUser, "Supplier");
                }
            }
            else
            {
                await userManager.AddToRoleAsync(supplierUser, "Supplier");
            }
            var supplier2Email = "supplier2@example.com";
var supplier2User = await userManager.FindByEmailAsync(supplier2Email);
if (supplier2User == null)
{
    supplier2User = new AppUser
    {
        UserName = supplier2Email,
        Email = supplier2Email,
        FullName = "İkinci Tedarikçi",
        EmailConfirmed = true,
        Address = "Tedarikçi Adresi",
        
    };
    var result2 = await userManager.CreateAsync(supplier2User, "Supplier123!");
    if (result2.Succeeded)
    {
        await userManager.AddToRoleAsync(supplier2User, "Supplier");
    }
}
else
{
    await userManager.AddToRoleAsync(supplier2User, "Supplier");
}


            // WarehouseManager Kullanıcı
            var warehouseEmail = "depo@eticaret.com";
            var warehouseUser = await userManager.FindByEmailAsync(warehouseEmail);
            if (warehouseUser == null)
            {
                warehouseUser = new AppUser
                {
                    UserName = warehouseEmail,
                    Email = warehouseEmail,
                    FullName = "Depo Yöneticisi",
                    Address = "Depo Adresi",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(warehouseUser, "Depo123*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(warehouseUser, "WarehouseManager");
                }
            }

            // Admin Kullanıcı
            var adminEmail = "admin@mvcproject.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin Kullanıcı",
                    Address = "Admin Adresi",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Normal Kullanıcı
            var normalUserEmail = "user@mvcproject.com";
            var normalUser = await userManager.FindByEmailAsync(normalUserEmail);
            if (normalUser == null)
            {
                normalUser = new AppUser
                {
                    UserName = normalUserEmail,
                    Email = normalUserEmail,
                    FullName = "Normal Kullanıcı",
                    Address = "Kullanıcı Adresi",
                    EmailConfirmed = true
                };
                var resultUser = await userManager.CreateAsync(normalUser, "User123!");
                if (resultUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }

            // Ürün ekleme
            if (!context.Products.Any())
            {
                var elektronikId = context.Categories.First(c => c.Name == "Elektronik").Id;
                var modaId = context.Categories.First(c => c.Name == "Moda").Id;
                var evId = context.Categories.First(c => c.Name == "Ev & Yaşam").Id;
                var kozmetikId = context.Categories.First(c => c.Name == "Kozmetik").Id;
                var sporCategory = context.Categories.FirstOrDefault(c => c.Name == "Spor & Outdoor");
if (sporCategory == null)
{
    throw new Exception("Spor & Outdoor kategorisi bulunamadı. Seed işlemi başarısız.");
}
var sporId = sporCategory.Id;
                context.Products.AddRange(new[]
                {
                    new Product
                    {
                        Name = "iPhone 15 Pro 128GB",
                        Description = "Apple A17 Pro, 6.1'' OLED, 48MP Kamera",
                        Price = 69999,
                        Stock = 20,
                        CategoryId = elektronikId,
                        ImageUrl = "https://cdn.vatanbilgisayar.com/Upload/PRODUCT/apple/thumb/132694-1_large.jpg",
                        SupplierId = supplierUser.Id
                    },
                    // Diğer ürünler...
                    new Product
                    {
                        Name = "Samsung Galaxy S24 Ultra",
                        Description = "200MP Kamera, 12GB RAM, 6.8'' AMOLED",
                        Price = 59999,
                        Stock = 15,
                        CategoryId = elektronikId,
                        ImageUrl = "https://cdn.vatanbilgisayar.com/Upload/PRODUCT/samsung/thumb/137123-1_large.jpg",
                        SupplierId = supplierUser.Id
                    },
                     new Product { Name = "MacBook Air M2 13''", Description = "Apple M2, 8GB RAM, 256GB SSD, Uzay Grisi", Price = 42999, Stock = 10, CategoryId = elektronikId, ImageUrl = "https://cdn.vatanbilgisayar.com/Upload/PRODUCT/apple/thumb/132694-1_large.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "AirPods Pro 2. Nesil", Description = "Aktif Gürültü Engelleme, MagSafe Kılıf", Price = 7999, Stock = 30, CategoryId = elektronikId, ImageUrl = "https://cdn.vatanbilgisayar.com/Upload/PRODUCT/apple/thumb/132694-1_large.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Xiaomi Mi Band 8", Description = "1.62'' AMOLED, 16 Gün Pil, 150+ Spor Modu", Price = 1299, Stock = 50, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Sony WH-1000XM5", Description = "Kablosuz Kulaklık, ANC, 30 Saat Pil", Price = 9999, Stock = 25, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Apple Watch Series 9", Description = "GPS, 41mm, Alüminyum Kasa", Price = 17999, Stock = 18, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "JBL Flip 6", Description = "Taşınabilir Bluetooth Hoparlör, Su Geçirmez", Price = 3499, Stock = 40, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Kindle Paperwhite 11. Nesil", Description = "6.8'' Ekran, 8GB, Suya Dayanıklı", Price = 4999, Stock = 12, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Logitech MX Master 3S", Description = "Kablosuz Mouse, 8000 DPI, Sessiz Tıklama", Price = 2999, Stock = 35, CategoryId = elektronikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
                     
    new Product { Name = "Nike Air Force 1", Description = "Beyaz, Unisex, Deri Sneaker", Price = 3499, Stock = 40, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Adidas Superstar", Description = "Siyah-Beyaz, Unisex, Klasik Sneaker", Price = 2999, Stock = 35, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Levi’s 511 Slim Jean", Description = "Erkek, Mavi, Pamuklu Kot Pantolon", Price = 1899, Stock = 25, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Mango Kadın Elbise", Description = "Çiçekli, Midi, Viskon Kumaş", Price = 1299, Stock = 30, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "LC Waikiki Basic Tişört", Description = "Beyaz, Pamuklu, Erkek", Price = 199, Stock = 100, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Koton Kışlık Mont", Description = "Erkek, Siyah, Su Geçirmez", Price = 1299, Stock = 20, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Zara Kadın Omuz Çantası", Description = "Hakiki Deri, Siyah", Price = 899, Stock = 15, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Puma Spor Ayakkabı", Description = "Erkek, Nefes Alabilir, Koşu", Price = 1599, Stock = 22, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Mavi Erkek Gömlek", Description = "Slim Fit, Pamuklu, Mavi", Price = 499, Stock = 40, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Defacto Kadın Bluz", Description = "Kısa Kollu, Beyaz, Viskon", Price = 299, Stock = 35, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Columbia Outdoor Ceket", Description = "Su Geçirmez, Erkek, Siyah", Price = 2499, Stock = 18, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Twist Kadın Etek", Description = "Midi, Siyah, Şifon", Price = 799, Stock = 20, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "H&M Çocuk Tişört", Description = "Pamuklu, Renkli, 2'li Paket", Price = 199, Stock = 60, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Pull&Bear Oversize Sweatshirt", Description = "Unisex, Siyah, Kapüşonlu", Price = 599, Stock = 30, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Mudo Kadın Trençkot", Description = "Bej, Kemerli, Uzun", Price = 1499, Stock = 15, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Kinetix Erkek Spor Ayakkabı", Description = "Koşu, Siyah, Hafif", Price = 399, Stock = 50, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Stradivarius Kadın Pantolon", Description = "Yüksek Bel, Siyah, Dar Paça", Price = 699, Stock = 25, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Bershka Erkek Şort", Description = "Pamuklu, Gri, Rahat Kesim", Price = 299, Stock = 40, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Vakko İpek Fular", Description = "Kadın, Desenli, Renkli", Price = 999, Stock = 10, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Lumberjack Erkek Bot", Description = "Kahverengi, Deri, Su Geçirmez", Price = 1299, Stock = 18, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Koton Kadın Kazak", Description = "Oversize, Bej, Yün Karışımlı", Price = 399, Stock = 22, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Mavi Erkek Mont", Description = "Kapitone, Siyah, Kışlık", Price = 999, Stock = 12, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Twist Kadın Blazer Ceket", Description = "Oversize, Siyah, Şık", Price = 1199, Stock = 14, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Defacto Erkek Eşofman Takımı", Description = "Pamuklu, Gri, 2 Parça", Price = 499, Stock = 30, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "Zara Kadın Topuklu Ayakkabı", Description = "Siyah, Deri, 7cm Topuk", Price = 799, Stock = 20, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product { Name = "LC Waikiki Çocuk Eşofman Altı", Description = "Pamuklu, Mavi, Rahat", Price = 149, Stock = 60, CategoryId = modaId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId = supplierUser.Id },
    new Product
{
    Name = "Ahşap Kitaplık",
    Description = "Modern tasarımlı, 5 raflı ahşap kitaplık",
    Price = 1299,
    Stock = 20,
    CategoryId = evId,
    ImageUrl = "https://cdn.dsmcdn.com/ty768/product/media/images/20230327/12/312859693/101325543/1/1_org.jpg",
    SupplierId = supplierUser.Id
},
new Product
{
    Name = "Seramik Vazo",
    Description = "El yapımı dekoratif seramik vazo - Beyaz",
    Price = 249,
    Stock = 50,
    CategoryId = evId,
    ImageUrl = "https://cdn.dsmcdn.com/ty892/product/media/images/20230512/11/345238012/110202303/1/1_org.jpg",
    SupplierId = supplierUser.Id
},
new Product
{
    Name = "6 Parça Tencere Seti",
    Description = "Paslanmaz çelik, endüksiyon uyumlu tencere seti",
    Price = 899,
    Stock = 15,
    CategoryId = evId,
    ImageUrl = "https://cdn.dsmcdn.com/ty1039/product/media/images/prod/SPM/PIM/20231025/09/fd91e3ef-2c53-3b13-bb0e-6bb701aeb35d/1_org.jpg",
    SupplierId = supplierUser.Id
},
new Product
{
    Name = "Pamuklu Nevresim Takımı",
    Description = "Çift kişilik, %100 pamuk, desenli nevresim seti",
    Price = 649,
    Stock = 30,
    CategoryId = evId,
    ImageUrl = "https://cdn.dsmcdn.com/ty1042/product/media/images/prod/SPM/PIM/20231025/08/50cdd018-7996-355f-aad2-4b0bdfc2a56c/1_org.jpg",
    SupplierId = supplierUser.Id
},

     new Product { Name = "L'Oréal Paris Nemlendirici Krem", Description = "Hyaluronik Asitli, 50ml, Tüm Cilt Tipleri", Price = 299, Stock = 100, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nivea Men Deodorant", Description = "48 Saat Etkili, 150ml, Roll-on", Price = 99, Stock = 80, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Maybelline Lash Sensational Maskara", Description = "Siyah, Yoğun Hacim", Price = 249, Stock = 60, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "MAC Retro Matte Ruj", Description = "Ruby Woo, Kırmızı, Mat", Price = 499, Stock = 40, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Garnier Micellar Temizleme Suyu", Description = "400ml, Tüm Ciltler", Price = 129, Stock = 70, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Bioderma Sensibio H2O", Description = "500ml, Hassas Ciltler", Price = 299, Stock = 50, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Neutrogena Hydro Boost Jel", Description = "50ml, Yoğun Nemlendirici", Price = 199, Stock = 60, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Urban Care Saç Bakım Yağı", Description = "100ml, Argan Yağlı", Price = 149, Stock = 40, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "La Roche-Posay Güneş Kremi", Description = "SPF 50+, 50ml, Hassas Ciltler", Price = 399, Stock = 30, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Dove Beauty Bar Sabun", Description = "4'lü Paket, Nemlendirici", Price = 89, Stock = 100, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "L'Oréal Paris Elseve Şampuan", Description = "400ml, Onarıcı, Kuru Saçlar", Price = 129, Stock = 80, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Pantene Pro-V Saç Kremi", Description = "200ml, Onarıcı, Tüm Saç Tipleri", Price = 99, Stock = 60, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Rexona Kadın Deodorant", Description = "150ml, Fresh, Sprey", Price = 89, Stock = 70, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nuxe Huile Prodigieuse Kuru Yağ", Description = "100ml, Çok Amaçlı", Price = 499, Stock = 30, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Bepanthol Cilt Bakım Kremi", Description = "100g, Nemlendirici", Price = 149, Stock = 50, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Bioderma Atoderm Dudak Balmı", Description = "15ml, Onarıcı", Price = 99, Stock = 40, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Maybelline Fit Me Fondöten", Description = "30ml, Mat, 110 Porcelain", Price = 199, Stock = 60, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Golden Rose Oje", Description = "Kırmızı, 10ml, Uzun Süre Kalıcı", Price = 39, Stock = 100, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "L'Oréal Paris True Match Kapatıcı", Description = "6ml, 1N Ivory", Price = 129, Stock = 80, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Garnier Saf & Temiz Peeling", Description = "150ml, Siyah Nokta Karşıtı", Price = 79, Stock = 70, CategoryId = kozmetikId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        // Spor & Outdoor (20+ gerçekçi ürün)
                        new Product { Name = "Adidas Duramo SL Koşu Ayakkabısı", Description = "Erkek, Hafif, Siyah", Price = 1799, Stock = 40, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nike Brasilia Spor Çanta", Description = "Unisex, 60L, Siyah", Price = 799, Stock = 30, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Decathlon Yoga Matı 8mm", Description = "Kaymaz, Mor, 173x61cm", Price = 399, Stock = 25, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Columbia Powder Lite Mont", Description = "Kadın, Outdoor, Siyah", Price = 2999, Stock = 15, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Salomon Speedcross 5", Description = "Outdoor Koşu Ayakkabısı, Erkek", Price = 3499, Stock = 12, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Puma Essentials Eşofman Altı", Description = "Erkek, Siyah, Pamuklu", Price = 599, Stock = 50, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "The North Face Base Camp Duffel", Description = "Unisex, 50L, Sarı", Price = 2499, Stock = 10, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Reebok Training Eldiveni", Description = "Unisex, Siyah, Ağırlık", Price = 299, Stock = 35, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Under Armour UA Tech Tişört", Description = "Erkek, Nefes Alabilir, Gri", Price = 399, Stock = 60, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Quechua Katlanır Kamp Sandalyesi", Description = "Mavi, Hafif, Taşınabilir", Price = 499, Stock = 20, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Decathlon Şişme Yatak", Description = "2 Kişilik, 140x200cm", Price = 899, Stock = 15, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nike Pro Erkek Tayt", Description = "Siyah, Nefes Alabilir", Price = 499, Stock = 30, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Columbia Kadın Outdoor Pantolon", Description = "Hafif, Hızlı Kuruyan", Price = 799, Stock = 20, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Quechua 2 Kişilik Çadır", Description = "Su Geçirmez, Kolay Kurulum", Price = 1999, Stock = 10, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Adidas Erkek Şort", Description = "Pamuklu, Siyah, Rahat", Price = 299, Stock = 40, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nike Kadın Spor Sütyeni", Description = "Destekli, Siyah, XS-L", Price = 399, Stock = 25, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Puma Unisex Sırt Çantası", Description = "Siyah, 25L, Suya Dayanıklı", Price = 499, Stock = 30, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Reebok Kadın Tayt", Description = "Yüksek Bel, Siyah, Esnek", Price = 399, Stock = 20, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Quechua Trekking Baton", Description = "Ayarlanabilir, Hafif, 2'li", Price = 299, Stock = 40, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id },
                        new Product { Name = "Nike Erkek Antrenman Eldiveni", Description = "Siyah, Nefes Alabilir", Price = 199, Stock = 50, CategoryId = sporId, ImageUrl = "https://cdn.dsmcdn.com/ty1000/product/media/images/20231001/13/418857209/968857857/1/1_org.jpg", SupplierId=supplier2.Id }
                       


                    
                });

                await context.SaveChangesAsync();
            }
        }
    }
}

                       
                       
              
      
