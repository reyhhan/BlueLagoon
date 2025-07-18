﻿using BlueLagoon.Application.Common.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        public VillaSuiteController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villaSuiteList = _unitOfWork.VillaSuite.GetAll(includeProperties:"Villa").ToList();
            return View(villaSuiteList);
        }

        public IActionResult Create()
        {
            VillaSuiteVM villaSuiteVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
            bool roomExists = _unitOfWork.VillaSuite.Any(u => u.VillaSuitId == obj.VillaSuite.VillaSuitId);

            if (ModelState.IsValid && !roomExists)
            {
                _unitOfWork.VillaSuite.Add(obj.VillaSuite);
                _unitOfWork.Save();
                TempData["success"] = "The suite has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            if (roomExists)
            {
                TempData["error"] = "Suite number already exists";
            }
            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                VillaSuite = _unitOfWork.VillaSuite.Get(u => u.VillaSuitId == villaSuiteId)
            };

            if (obj is null)
            {
                return View(obj);    
            }
            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                _unitOfWork.VillaSuite.Update(villaSuiteVM.VillaSuite);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }          
            TempData["error"] = "The villa could not be updated!";
            villaSuiteVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
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
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaSuite = _unitOfWork.VillaSuite.Get(u => u.VillaSuitId == villaSuiteId)
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
            VillaSuite villaSuite = _unitOfWork.VillaSuite.Get(c => c.VillaSuitId == villaSuiteVM.VillaSuite.VillaSuitId);

            if (villaSuite is not null)
            {
                _unitOfWork.VillaSuite.Delete(villaSuite);
                _unitOfWork.Save();
                TempData["success"] = "The suite has been successfully removed!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["error"] = "The suite could not be deleted!"; 
            return View(villaSuiteVM);
        }

    }
}
