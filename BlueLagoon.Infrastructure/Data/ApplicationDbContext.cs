using BlueLagoon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlueLagoon.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "SunRay",
                    Description = "Near the beach",
                    ImageUrl = "",
                    Occupancy = 4,
                    Price = 10000,
                    Sqft = 500
                });
        }
    }
}
