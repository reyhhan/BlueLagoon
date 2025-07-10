using System.Diagnostics;
using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Web.Models;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties : "VillaAmenities"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };
                
            return View(homeVM);
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
