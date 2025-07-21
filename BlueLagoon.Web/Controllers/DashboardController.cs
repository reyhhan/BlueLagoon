using BlueLagoon.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialBarChart()
        {          
            return Json(await _dashboardService.GetTotalBookingRadialBarChart());      
        }

        public async Task<IActionResult> GetTotalUsersRadialBarChart()
        {
            return Json(await _dashboardService.GetTotalUsersRadialBarChart());
        }


        public async Task<IActionResult> GetRevenueRadialBarChart()
        {
            return Json(await _dashboardService.GetRevenueRadialBarChart());
        }

        public async Task<IActionResult> GetBookingsPieChart()
        {
            return Json(await _dashboardService.GetBookingsPieChart());

        }

        public async Task<IActionResult> GetMemberAndBookingLineBarChart()
        {
            return Json(await _dashboardService.GetMemberAndBookingLineBarChart());
        }
    }
}
