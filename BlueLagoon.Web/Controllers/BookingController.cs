using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BlueLagoon.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BookingDetails(int bookingId)
        {
            Booking booking = _unitOfWork.Booking.Get(u => u.BookingId == bookingId, includeProperties: "User,Villa");
            if(booking.VillaSuite == 0 && booking.Status == Constants.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaSuiteByVilla(booking.VillaId);
                booking.VillaSuites = _unitOfWork.VillaSuite.GetAll(u => u.VillaId == booking.VillaId && availableVillaNumber.Any(x => x == u.VillaSuitId)).ToList(); 
            }
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.BookingId, Constants.StatusCheckedIn, booking.VillaSuite);
            _unitOfWork.Save();
            TempData["Success"] = "This booking has been updated successfully";
            return RedirectToAction(nameof(BookingDetails), new {bookingId = booking.BookingId });
        }


        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.BookingId, Constants.StatusCompleted, booking.VillaSuite);
            _unitOfWork.Save();
            TempData["Success"] = "This booking has been completed upon checkout";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.BookingId });
        }

        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.BookingId, Constants.StatusCancelled, 0);
            _unitOfWork.Save();
            TempData["Success"] = "This booking has been cancelled";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.BookingId });
        }
        private List<int> AssignAvailableVillaSuiteByVilla(int villaId)
        {
            List<int> availableVillaSuites = new();

            var villaSuites = _unitOfWork.VillaSuite.GetAll(u => u.VillaId == villaId);

            var bookedVillaSuites = _unitOfWork.Booking.GetAll(u => u.VillaId == villaId && u.Status == Constants.StatusCheckedIn)
                .Select(u => u.VillaSuite);

            foreach (var villaSuite in villaSuites)
            {
                if (!bookedVillaSuites.Contains(villaSuite.VillaSuitId))
                {
                    availableVillaSuites.Add(villaSuite.VillaSuitId);
                }
            }
            return availableVillaSuites;

        }

        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u=>u.Id == userId);
            Booking booking = new()
            {
                VillaId = villaId,
                Villa = _unitOfWork.Villa.Get(u => u.Id == villaId, includeProperties: "VillaAmenities"),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Email = user.Email,
                Name = user.Name,
                Phone = user.PhoneNumber
            };

            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var villa = _unitOfWork.Villa.Get(u => u.Id == booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = Constants.StatusPending;
            booking.BookingDate = DateTime.Now;

 
            var villaSuites = _unitOfWork.VillaSuite.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == Constants.StatusApproved || u.Status == Constants.StatusCheckedIn).ToList();
        
            int roomsAvaialble = Constants.VillaRoomsAvailable_Count(villa.Id, villaSuites, booking.CheckInDate, booking.Nights, bookedVillas);

            if(roomsAvaialble == 0)
            {
                TempData["error"] = "Rooms not available";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkIndate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }
           
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.BookingId}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
                CancelUrl = domain + $"booking/FinalizeBooking?villaId={booking.VillaId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}"

            };

            options.LineItems.Add(new Stripe.Checkout.SessionLineItemOptions
            {
                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)booking.TotalCost * 100,
                    Currency = "usd",
                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                    },
                },
                Quantity = 1
            });

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            _unitOfWork.Booking.UpdateStripePaymentID(booking.BookingId, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {

            Booking booking = _unitOfWork.Booking.Get(u => u.BookingId == bookingId, includeProperties: "User,Villa");

            if(booking.Status == Constants.StatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);

                if(session.PaymentStatus == "paid")
                {
                    //confirm is successful payment
                    _unitOfWork.Booking.UpdateStatus(bookingId, Constants.StatusApproved, 0);
                    _unitOfWork.Booking.UpdateStripePaymentID(booking.BookingId, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
            return View(bookingId);
        }


        #region API CALLS
        [HttpGet]
        [Authorize(Roles = Constants.Role_Admin + "," + Constants.Role_Customer)]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;

            if (User.IsInRole(Constants.Role_Admin))
            {
                objBookings = _unitOfWork.Booking.GetAll(includeProperties : "User,Villa");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Villa");
            }

            if (!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));
            }
            return Json(new { data = objBookings });    
        }



        #endregion
    }
}
