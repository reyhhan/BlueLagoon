using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

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

        //[HttpPost]
        //public IActionResult Index(HomeVM homeVM)
        //{
        //    homeVM.VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities");
        //    foreach (var villa in homeVM.VillaList)
        //    {
        //        if (villa.Id % 2 == 0)
        //        {
        //            villa.IsAvailable = false;
        //        }
        //    }         
        //    return View(homeVM);
        //}

        [HttpPost]  
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities");
            var villaSuites = _unitOfWork.VillaSuite.GetAll().ToList();

            var bookedVillas =  _unitOfWork.Booking.GetAll(u=>u.Status == Constants.StatusApproved || u.Status == Constants.StatusCheckedIn).ToList();
            foreach (var villa in villaList)
            {
                int roomsAvaialble= Constants.VillaRoomsAvailable_Count(villa.Id, villaSuites, checkInDate, nights, bookedVillas);

                villa.IsAvailable = roomsAvaialble > 0 ? true : false;
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = villaList,
                Nights = nights
            };
            return PartialView("_VillaList", homeVM);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
