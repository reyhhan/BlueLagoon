using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVillaService _villaService;

        public HomeController(IVillaService villaService)
        {            
            _villaService = villaService;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _villaService.GetAllVillas(),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };
                
            return View(homeVM);
        }

       
        [HttpPost]  
        public IActionResult GetVillasByDate(int nights, DateOnly checkInDate)
        {      
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                VillaList = _villaService.GetVillasAvailabilityByDate(nights, checkInDate),
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
