using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    [Authorize]
    public class VillaController : Controller
    {
        private readonly IVillaService _villaService;
        public VillaController(IVillaService villaService)
        {
           _villaService = villaService;    
        }
        public IActionResult Index()
        {
            var villaList = _villaService.GetAllVillas();
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
                _villaService.CreateVilla(obj);
              
                TempData["success"] = "The villa has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be created!";
            return View();
            
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _villaService.GetVillaById(villaId);
            if (obj == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Update(Villa villa)
        {
            if (ModelState.IsValid && villa.Id > 0)
            {
                _villaService.UpdateVilla(villa);
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be updated!";
            return View(villa);
        }
        
        public IActionResult Delete(int villaId)
        {
            bool delete = _villaService.DeleteVilla(villaId);

            if (delete)
            {
                TempData["success"] = "The villa has been successfully deleted!";
                return RedirectToAction(nameof(Index));
            }                           
            else
            {
                TempData["error"] = "The villa could not be deleted!";
            }
            return View();
        }
    }
}
