using BlueLagoon.Domain.Entities;

namespace BlueLagoon.Application.Services.Interface
{
    public interface IBookingService
    {
        void CreateBooking(Booking booking);
        Booking GetBookingById(int bookingId);
        IEnumerable<Booking> GetAllBooking(string userId = "", string? stringFilterList = "");
        void UpdateStatus(int bookingId, string bookingStatus, int villaSuite = 0);
        void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
        IEnumerable<int> GetCheckedInVillaNumber(int villaId);
    }
}
