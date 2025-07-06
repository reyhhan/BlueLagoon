using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VillaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var villaList = _context.Villas.ToList();
            return View(villaList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                ModelState.AddModelError("", "Name and Description cannot be same");
            }
            if (ModelState.IsValid)
            {
                _context.Villas.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "The villa has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be created!";
            return View();
            
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _context.Villas.FirstOrDefault(u => u.Id == villaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");       
            }
           
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa obj)
        {           
            if (ModelState.IsValid && obj.Id > 0)
            {
                _context.Villas.Update(obj);
                _context.SaveChanges();
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be updated!";
            return View();
        }
       
    
        public IActionResult Delete(int villaId)
        {      
            if (villaId > 0)
            {
                Villa? obj = _context.Villas.Find(villaId);
                if (obj is not null) 
                {
                    _context.Villas.Remove(obj);
                    _context.SaveChanges();
                    TempData["success"] = "The villa has been successfully deleted!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = "The villa could not be deleted!";
            }
            return View();
        }
    }
}
