using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;

namespace BlueLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Booking booking)
        {
            _db.Bookings.Update(booking);
        }

        public void UpdateStatus(int bookingId, string bookingStatus, int villaSuite = 0)
        {
            var booking = _db.Bookings.FirstOrDefault(m => m.BookingId == bookingId);
            if (booking != null)
            {
                booking.Status = bookingStatus;
                if (bookingStatus == Constants.StatusCheckedIn)
                {
                    booking.VillaSuite = villaSuite;
                    booking.ActualCheckInDate = DateTime.Now;
                }

                if (bookingStatus == Constants.StatusCompleted)
                {
                    booking.ActualCheckOutDate = DateTime.Now;
                }
            }
        }

        public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId)
        {
            var booking = _db.Bookings.FirstOrDefault(m => m.BookingId == bookingId);
            if (booking != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    booking.StripeSessionId = sessionId;
                }

                if (!string.IsNullOrEmpty(sessionId))
                {
                    booking.StripePaymentIntentId = paymentIntentId;
                    booking.PaymentDate = DateTime.Now;
                    booking.IsPaymentSuccessful = true;
                }
            }
        }
    }
}
