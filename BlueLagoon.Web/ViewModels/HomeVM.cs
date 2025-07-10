using BlueLagoon.Domain.Entities;

namespace BlueLagoon.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInDate {  get; set; }
        public DateOnly CheckOutDate { get; set; }
        public int Nights {  get; set; }
    }
}
