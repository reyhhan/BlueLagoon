using BlueLagoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking booking);
        void UpdateStatus(int bookingId, string bookingStatus, int villaSuite = 0);
        void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId);
    }
}
