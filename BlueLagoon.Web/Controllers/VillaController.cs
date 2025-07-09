using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");
                    using var filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                    
                    obj.Image.CopyTo(filestream);
                    obj.ImageUrl = @"\images\VillaImages\" + fileName;
                    
                }
                else
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }
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
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");
                   

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    using var filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                    obj.Image.CopyTo(filestream);
                    obj.ImageUrl = @"\images\VillaImages\" + fileName;

                }

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

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    
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
