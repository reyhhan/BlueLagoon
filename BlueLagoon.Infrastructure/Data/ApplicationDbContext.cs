using BlueLagoon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlueLagoon.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //Create villa table
        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Deluxe Villa",
                    Description = "Near the beach",
                    ImageUrl = "",
                    Occupancy = 4,
                    Price = 10000,
                    Sqft = 400
                },
                new Villa
                {
                    Id = 2,
                    Name = "Premium Pool Villa",
                    Description = "Pool View",
                    ImageUrl = "",
                    Occupancy = 4,
                    Price = 30000,
                    Sqft = 550
                },
                new Villa
                {
                    Id = 3,
                    Name = "Luxury Pool Villa",
                    Description = "Kind Size Bed",
                    ImageUrl = "",
                    Occupancy = 10,
                    Price = 80000,
                    Sqft = 1050
                });
        }
    }
}
