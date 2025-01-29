using Lab12.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Data
{
    public static class DbContextExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" },
                new Category { Id = 3, Name = "Fashion" },
                new Category { Id = 4, Name = "Home & Garden" },
                new Category { Id = 5, Name = "Health & Beauty" },
                new Category { Id = 6, Name = "Sports" }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = 1,
                    Name = "Xiaomi Redmi Note 13",
                    Price = 1299.99m,
                    ImagePath = null,
                    CategoryId = 1
                },
                new Article
                {
                    Id = 2,
                    Name = "E-book reader",
                    Price = 459.99m,
                    ImagePath = null,
                    CategoryId = 1
                },
                new Article
                {
                    Id = 3,
                    Name = "Computer",
                    Price = 4499.0m,
                    ImagePath = null,
                    CategoryId = 1
                },
                new Article
                {
                    Id = 4,
                    Name = "The Hobbit",
                    Price = 49.99m,
                    ImagePath = null,
                    CategoryId = 2
                },
                new Article
                {
                    Id = 5,
                    Name = "The Lord of the Rings",
                    Price = 89.99m,
                    ImagePath = null,
                    CategoryId = 2
                },
                new Article
                {
                    Id = 6,
                    Name = "T-shirt",
                    Price = 69.99m,
                    ImagePath = null,
                    CategoryId = 3
                },
                new Article
                {
                    Id = 7,
                    Name = "Hoodie",
                    Price = 159.99m,
                    ImagePath = null,
                    CategoryId = 3
                },
                new Article
                {
                    Id = 8,
                    Name = "Watering can",
                    Price = 29.99m,
                    ImagePath = null,
                    CategoryId = 4
                },
                new Article
                {
                    Id = 9,
                    Name = "Pot",
                    Price = 19.59m,
                    ImagePath = null,
                    CategoryId = 4
                },
                new Article
                {
                    Id = 10,
                    Name = "Lipstick",
                    Price = 9.99m,
                    ImagePath = null,
                    CategoryId = 5
                },
                new Article
                {
                    Id = 11,
                    Name = "Skateboard",
                    Price = 64.99m,
                    ImagePath = null,
                    CategoryId = 6
                },
                new Article
                {
                    Id = 12,
                    Name = "Basketball ball",
                    Price = 39.99m,
                    ImagePath = null,
                    CategoryId = 6
                }
            );
        }
    }
}
