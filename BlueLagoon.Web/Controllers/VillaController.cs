using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var villaList = _unitOfWork.Villa.GetAll();
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
                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been successfully created!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be created!";
            return View();
            
        }

        public IActionResult Update(int villaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == villaId);
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
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "The villa has been successfully updated!";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "The villa could not be updated!";
            return View(obj);
        }
       
    
        public IActionResult Delete(int villaId)
        {      
            if (villaId > 0)
            {
                Villa? obj = _unitOfWork.Villa.Get(u=>u.Id== villaId);
                if (obj is not null) 
                {
                    _unitOfWork.Villa.Delete(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "The villa has been successfully deleted!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = "The villa could not be deleted!";
            }
            return View();
        }
    }
}
