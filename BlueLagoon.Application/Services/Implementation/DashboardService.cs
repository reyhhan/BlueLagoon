using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Web.ViewModels;


namespace BlueLagoon.Application.Services.Implementation
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, previousMonth, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);


        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PieChartDto> GetBookingsPieChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) &&
           (u.Status != Constants.StatusPending || u.Status == Constants.StatusCancelled));

            var customerWithOneBooking = totalBookings.GroupBy(b => b.UserId).Where(x => x.Count() == 1).Select(x => x.Key).ToList();

            int bookingsByNewCustomer = customerWithOneBooking.Count;
            int bookingsByReturningCustomer = totalBookings.Count() - bookingsByNewCustomer;

            PieChartDto pieChartDto = new()
            {
                Labels = new string[] { "New Customer Bookings", "Returning Customer Bookings" },
                Series = new decimal[] { bookingsByNewCustomer, bookingsByReturningCustomer }
            };

            return pieChartDto;
        }

        public async Task<LineChartDto> GetMemberAndBookingLineBarChart()
        {
            var bookingData = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && u.BookingDate.Date <= DateTime.Now)
                 .GroupBy(b => b.BookingDate.Date)
                 .Select(m => new
                 {
                     DateTime = m.Key,
                     NewBookingCount = m.Count(),
                 });

            var customerData = _unitOfWork.ApplicationUser.GetAll(u => u.CreatedAt >= DateTime.Now.AddDays(-30) && u.CreatedAt.Date <= DateTime.Now)
                .GroupBy(b => b.CreatedAt.Date)
                .Select(m => new
                {
                    DateTime = m.Key,
                    NewUserCount = m.Count(),
                });

            var leftJoin = bookingData.GroupJoin(customerData, booking => booking.DateTime, customer => customer.DateTime,
                (booking, customer) => new
                {
                    booking.DateTime,
                    booking.NewBookingCount,
                    NewUserCount = customer.Select(x => x.NewUserCount).FirstOrDefault()
                }).ToList();


            var rightJoin = customerData.GroupJoin(bookingData, customer => customer.DateTime, booking => booking.DateTime,
                (customer, booking) => new
                {
                    customer.DateTime,
                    NewBookingCount = booking.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewUserCount
                }).ToList();

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newBookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newCustomerData = mergedData.Select(x => x.NewUserCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("MM/dd/yyyy")).ToArray();

            LineChartDto lineChartVM = new()
            {
                Categories = categories,
            };

            List<ChartData> charDataList = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data = newBookingData
                },
                 new ChartData
                {
                    Name = "New Members",
                    Data = newCustomerData
                }
            };
            lineChartVM.Series = charDataList;

            return lineChartVM;

        }

        public async Task<RadialBarChartDto> GetRevenueRadialBarChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != Constants.StatusPending
            || u.Status != Constants.StatusCancelled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(c => c.TotalCost));

            var countByCurrentMonth = totalBookings.Where(c => c.BookingDate >= currentMonthStartDate
            && c.BookingDate <= DateTime.Now).Sum(c => c.TotalCost);

            var countByPreviousMonth = totalBookings.Where(c => c.BookingDate >= previousMonthStartDate
           && c.BookingDate <= currentMonthStartDate).Sum(c => c.TotalCost);


            return GetRadialBarChartData(totalRevenue, countByCurrentMonth, countByPreviousMonth);
        }

        public async Task<RadialBarChartDto> GetTotalBookingRadialBarChart()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != Constants.StatusPending
            || u.Status != Constants.StatusCancelled);

            var countByCurrentMonth = totalBookings.Count(c => c.BookingDate >= currentMonthStartDate
            && c.BookingDate <= DateTime.Now);

            var countByPreviousMonth = totalBookings.Count(c => c.BookingDate >= previousMonthStartDate
           && c.BookingDate <= currentMonthStartDate);

            return GetRadialBarChartData(totalBookings.Count(), countByCurrentMonth, countByPreviousMonth);

        }

        public async Task<RadialBarChartDto> GetTotalUsersRadialBarChart()
        {
            var totalUsers = _unitOfWork.ApplicationUser.GetAll();

            var countByCurrentMonth = totalUsers.Count(c => c.CreatedAt >= currentMonthStartDate
            && c.CreatedAt <= DateTime.Now);

            var countByPreviousMonth = totalUsers.Count(c => c.CreatedAt >= previousMonthStartDate
           && c.CreatedAt <= currentMonthStartDate);

            return GetRadialBarChartData(totalUsers.Count(), countByCurrentMonth, countByPreviousMonth );
        }


        private static RadialBarChartDto GetRadialBarChartData(int count, double countByCurrentMonth, double countByPreviousMonth)
        {
            int increaseRatio = 100;
            RadialBarChartDto radialBarChartVM = new RadialBarChartDto();
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
