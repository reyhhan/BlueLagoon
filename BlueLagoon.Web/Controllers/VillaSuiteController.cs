using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
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
        private readonly IVillaSuiteService _villaSuiteService;
        private readonly IVillaService _villaService;

        public VillaSuiteController(IVillaSuiteService villaSuiteService, IVillaService villaService)
        {
            _villaSuiteService = villaSuiteService;
            _villaService = villaService;
        }
        public IActionResult Index()
        {
            var villaSuiteList = _villaSuiteService.GetAllVillaSuites();
            return View(villaSuiteList);
        }

        public IActionResult Create()
        {
            VillaSuiteVM villaSuiteVM = new()
            {
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
            bool roomExists =_villaSuiteService.CheckVillaSuiteExits(obj.VillaSuite.VillaSuitId);

            if (ModelState.IsValid && !roomExists)
            {
                _villaSuiteService.CreateVillaSuite(obj.VillaSuite);
                TempData["success"] = "The suite has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            if (roomExists)
            {
                TempData["error"] = "Suite number already exists";
            }
            obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
                VillaSuite = _villaSuiteService.GetVillaSuiteById(villaSuiteId)
            };

            if (obj is null)
            {
                return View(obj);    
            }
            obj.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
                _villaSuiteService.UpdateVillaSuite(villaSuiteVM.VillaSuite);
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }          
            TempData["error"] = "The villa could not be updated!";
            villaSuiteVM.VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
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
                VillaList = _villaService.GetAllVillas().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaSuite = _villaSuiteService.GetVillaSuiteById(villaSuiteId)
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
            VillaSuite villaSuite = _villaSuiteService.GetVillaSuiteById(villaSuiteVM.VillaSuite.VillaSuitId);

            if (villaSuite is not null)
            {
                _villaSuiteService.DeleteVillaSuite(villaSuiteVM.VillaSuite.VillaSuitId);
                TempData["success"] = "The suite has been successfully removed!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["error"] = "The suite could not be deleted!"; 
            return View(villaSuiteVM);
        }

    }
}
