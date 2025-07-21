namespace BlueLagoon.Web.ViewModels
{
    public class RadialBarChartDto
    {
        public decimal TotalCount { get; set; }
        public decimal CountByCurrentMonth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }


    }
}
