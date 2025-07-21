using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
                if (!_roleManager.RoleExistsAsync(Constants.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(Constants.Role_Admin)).Wait();
                    _roleManager.CreateAsync(new IdentityRole(Constants.Role_Customer)).Wait();
                }

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    Email = "adminone@gmail.com",
                    PhoneNumber = "0777738499",
                    Name = "Admin User",
                    NormalizedEmail = "admin",
                    NormalizedUserName = "adminone@gmail.com",
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "adminone@gmail.com");
                _userManager.AddToRoleAsync(user, Constants.Role_Admin).GetAwaiter().GetResult();

            }
            catch
            {
                throw;
            }
          
        }
    }
}
