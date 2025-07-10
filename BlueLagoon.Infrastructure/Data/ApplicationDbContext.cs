using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlueLagoon.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //Create villa table
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaSuite> VillaSuites { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

            modelBuilder.Entity<VillaSuite>().HasData(
                new VillaSuite
                {
                    VillaSuitId = 101,
                    VillaId = 1,
                },
                new VillaSuite
                {
                    VillaSuitId = 102,
                    VillaId = 1,
                },
                new VillaSuite
                {
                    VillaSuitId = 103,
                    VillaId = 1,
                },
                new VillaSuite
                {
                    VillaSuitId = 201,
                    VillaId = 2,
                },
                new VillaSuite
                {
                    VillaSuitId = 202,
                    VillaId = 2,
                },
                new VillaSuite
                {
                    VillaSuitId = 203,
                    VillaId = 2,
                },
                new VillaSuite
                {
                    VillaSuitId = 301,
                    VillaId = 3,
                },
                new VillaSuite
                {
                    VillaSuitId = 302,
                    VillaId = 3,
                },
                new VillaSuite
                {
                    VillaSuitId = 303,
                    VillaId = 3,
                }
            );

            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    Id =1,
                    Name = "Pool",
                    Description = "xx",
                    VillaId = 13
                },
                 new Amenity
                 {
                     Id =2,
                     Name = "Jaquse",
                     Description = "De",
                     VillaId = 13
                 },
                  new Amenity
                  {
                      Id = 3,
                      Name = "Balcony",
                      Description = "De",
                      VillaId = 2
                  },
                   new Amenity
                   {
                       Id = 4,
                       Name = "Beach View",
                       Description = "De",
                       VillaId = 2
                   },
                    new Amenity
                    {
                        Id = 5,
                        Name = "Terrace",
                        Description = "De",
                        VillaId = 3
                    }
                );
            
        }
    }
}
