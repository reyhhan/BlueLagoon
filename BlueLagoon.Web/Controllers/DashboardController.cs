using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlueLagoon.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth , 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);


        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialBarChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != Constants.StatusPending
            || u.Status != Constants.StatusCancelled);

            var countByCurrentMonth = totalBookings.Count(c => c.BookingDate >= currentMonthStartDate
            && c.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(c => c.BookingDate >= previousMonthStartDate
           && c.BookingDate <= currentMonthStartDate);


            return Json(GetRadialBarChartData(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth));

        }

        public async Task<IActionResult> GetTotalUsersRadialBarChart()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            var countByCurrentMonth = totalUsers.Count(c => c.CreatedAt >= currentMonthStartDate
            && c.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(c => c.CreatedAt >= previousMonthStartDate
           && c.CreatedAt <= currentMonthStartDate);

            

            return Json(GetRadialBarChartData(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth));
        }


        public async Task<IActionResult> GetRevenueRadialBarChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != Constants.StatusPending
            || u.Status != Constants.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(c => c.TotalCost));

            var countByCurrentMonth = totalBookings.Where(c => c.BookingDate >= currentMonthStartDate
            && c.BookingDate <= DateTime.Now).Sum(c=>c.TotalCost);

            var countByPreviousMonth = totalBookings.Where(c => c.BookingDate >= previousMonthStartDate
           && c.BookingDate <= currentMonthStartDate).Sum(c => c.TotalCost);


            return Json(GetRadialBarChartData(totalRevenue, countByCurrentMonth, countByPreviousMonth));
        }
        public async Task<IActionResult> GetBookingsPieChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
          (u.Status != Constants.StatusPending || u.Status == Constants.StatusCancelled));

            var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count;
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartDto PieChartDto = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return Json(PieChartDto);
        }

       
        private static RadialBarChartVM GetRadialBarChartData(int count, double countByCurrentMonth, double countByPreviousMonth)
        {
            int increaseRatio = 100;
            RadialBarChartVM radialBarChartVM = new RadialBarChartVM();
            if (countByPreviousMonth != 0)
                increaseRatio = Convert.ToInt32((countByCurrentMonth - countByPreviousMonth) / countByPreviousMonth * 100);


            radialBarChartVM.TotalCount = count;
            radialBarChartVM.CountByCurrentMonth = (decimal)countByCurrentMonth;
            radialBarChartVM.HasRatioIncreased = countByCurrentMonth > countByPreviousMonth;
            radialBarChartVM.Series = new int[] { increaseRatio };

            return radialBarChartVM;

        }
    }
}
