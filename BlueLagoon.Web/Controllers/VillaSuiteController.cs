using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlueLagoon.Web.Controllers
{
    public class VillaSuiteController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VillaSuiteController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villaSuiteList = _context.VillaSuites.Include(u=>u.Villa).ToList();
            return View(villaSuiteList);
        }

        public IActionResult Create()
        {
            VillaSuiteVM villaSuiteVM = new()
            {
                VillaList = _context.Villas.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
                   
            return View(villaSuiteVM);
        }

        [HttpPost]
        public IActionResult Create(VillaSuiteVM obj)
        {
            ModelState.Remove("Villa");
            bool roomExists = _context.VillaSuites.Any(u => u.VillaSuitId == obj.VillaSuite.VillaSuitId);
            
            if (ModelState.IsValid && !roomExists)
            {
                _context.VillaSuites.Add(obj.VillaSuite);
                _context.SaveChanges();
                TempData["success"] = "The suite has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            if (roomExists)
            {
                TempData["error"] = "Suite number already exists";
            }
            obj.VillaList = _context.Villas.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
            
        }

        public IActionResult Update(int villaSuiteId)
        {
            VillaSuiteVM obj = new()
            {
                VillaSuite = _context.VillaSuites.FirstOrDefault(u => u.VillaSuitId == villaSuiteId)
            };

            if (obj is null)
            {
                return View(obj);    
            }
            obj.VillaList = _context.Villas.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(VillaSuiteVM villaSuiteVM)
        {
            if (ModelState.IsValid)
            {
                _context.VillaSuites.Update(villaSuiteVM.VillaSuite);
                _context.SaveChanges();
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }          
            TempData["error"] = "The villa could not be updated!";
            villaSuiteVM.VillaList = _context.Villas.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaSuiteVM);
        }
       
    
        public IActionResult Delete(int villaSuiteId)
        {
            VillaSuiteVM villaSuiteVM = new()
            {
                VillaList = _context.Villas.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaSuite = _context.VillaSuites.FirstOrDefault(u => u.VillaSuitId == villaSuiteId)
            };
            if(villaSuiteVM.VillaSuite is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(villaSuiteVM);
        }

        [HttpPost]
        public IActionResult Delete(VillaSuiteVM villaSuiteVM)
        {
            VillaSuite villaSuite = _context.VillaSuites.FirstOrDefault(c => c.VillaSuitId == villaSuiteVM.VillaSuite.VillaSuitId);

            if (villaSuite is not null)
            {
                _context.VillaSuites.Remove(villaSuite);
                _context.SaveChanges();
                TempData["success"] = "The suite has been successfully removed!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["error"] = "The suite could not be deleted!"; 
            return View(villaSuiteVM);
        }

    }
}
