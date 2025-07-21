using BlueLagoon.Application.Services.Implementation;
using BlueLagoon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
