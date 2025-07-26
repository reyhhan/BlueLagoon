using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Services.Implementation
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public void CreateBooking(Booking booking)
        {
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();
        }

        public IEnumerable<Booking> GetAllBooking(string userId = "", string? stringFilterList = "")
        {
            IEnumerable<string> statusList = stringFilterList.ToLower().Split(",");
            if (!string.IsNullOrEmpty(stringFilterList) && !string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()) &&
                u.UserId == userId, includeProperties : "User,Villa");
            }
            else
            {
                if (!string.IsNullOrEmpty(stringFilterList))
                {
                    return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()), includeProperties: "User,Villa");
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    return _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Villa");

                }
            }
            return _unitOfWork.Booking.GetAll(includeProperties: "User,Villa");
        }

        public Booking GetBookingById(int bookingId)
        {
            return _unitOfWork.Booking.Get(u=>u.BookingId == bookingId ,includeProperties: "User,Villa");
        }

        public IEnumerable<int> GetCheckedInVillaNumber(int villaId)
        {
            return _unitOfWork.Booking.GetAll(u => u.VillaId == villaId && u.Status == Constants.StatusCheckedIn)
     .Select(u => u.VillaSuite);

        }

        public void UpdateStatus(int bookingId, string bookingStatus, int villaSuite = 0)
        {
            var booking = _unitOfWork.Booking.Get(m => m.BookingId == bookingId, tracked : true);
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

            _unitOfWork.Save();

        }

        public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId)
        {
            var booking = _unitOfWork.Booking.Get(m => m.BookingId == bookingId, tracked : true);
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

            _unitOfWork.Save();
        }
    }
}
