using BlueLagoon.Web.ViewModels;

namespace BlueLagoon.Application.Services.Interface
{
    public interface IDashboardService
    {
        Task<PieChartDto> GetBookingsPieChart();

        Task<LineChartDto> GetMemberAndBookingLineBarChart();

        Task<RadialBarChartDto> GetRevenueRadialBarChart();

        Task<RadialBarChartDto> GetTotalUsersRadialBarChart();

        Task<RadialBarChartDto> GetTotalBookingRadialBarChart();
    }
}
