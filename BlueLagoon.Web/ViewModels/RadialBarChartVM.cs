namespace BlueLagoon.Web.ViewModels
{
    public class RadialBarChartVM
    {
        public decimal TotalCount { get; set; }
        public decimal CountByCurrentMonth { get; set; }
        public bool HasRatioIncreased { get; set; }
        public int[] Series { get; set; }


    }
}
