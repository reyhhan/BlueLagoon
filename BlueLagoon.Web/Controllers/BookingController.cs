using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using System.Security.Claims;

namespace BlueLagoon.Web.Controllers
{
    public class BookingController : Controller
    {

        private readonly IVillaService _villaService;
        private readonly IVillaSuiteService _villaSuiteService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingController(IVillaService villaService,
            IVillaSuiteService villaSuiteService,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _villaService = villaService;
            _villaSuiteService = villaSuiteService;
            _bookingService = bookingService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BookingDetails(int bookingId)
        {
            Booking booking = _bookingService.GetBookingById(bookingId);

            if(booking.VillaSuite == 0 && booking.Status == Constants.StatusApproved)
            {
                var availableVillaNumber = AssignAvailableVillaSuiteByVilla(booking.VillaId);
                booking.VillaSuites = _villaSuiteService.GetAllVillaSuites().Where(u => u.VillaId == booking.VillaId
                && availableVillaNumber.Any(x => x == u.VillaSuitId)).ToList(); 
            }
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _bookingService.UpdateStatus(booking.BookingId, Constants.StatusCheckedIn, booking.VillaSuite);
           
            TempData["Success"] = "This booking has been updated successfully";
            return RedirectToAction(nameof(BookingDetails), new {bookingId = booking.BookingId });
        }


        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _bookingService.UpdateStatus(booking.BookingId, Constants.StatusCompleted, booking.VillaSuite);
       
            TempData["Success"] = "This booking has been completed upon checkout";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.BookingId });
        }

        [HttpPost]
        [Authorize(Roles = Constants.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _bookingService.UpdateStatus(booking.BookingId, Constants.StatusCancelled, 0);
           
            TempData["Success"] = "This booking has been cancelled";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.BookingId });
        }

        private List<int> AssignAvailableVillaSuiteByVilla(int villaId)
        {
            List<int> availableVillaSuites = new();

            var villaSuites = _villaSuiteService.GetAllVillaSuites().Where(u=>u.VillaId == villaId);

            var bookedVillas = _bookingService.GetCheckedInVillaNumber(villaId);

            foreach (var villaSuite in villaSuites)
            {
                if (!bookedVillas.Contains(villaSuite.VillaSuitId))
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

            ApplicationUser user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            Booking booking = new()
            {
                VillaId = villaId,
                Villa = _villaService.GetVillaById(villaId),
                CheckInDate = checkInDate,
                Nights = nights,
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
                Email = user.Email ?? string.Empty,
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
            var villa =_villaService.GetVillaById(booking.VillaId);
            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = Constants.StatusPending;
            booking.BookingDate = DateTime.Now;
         
            if(!_villaService.IsVillaAvailableByDate(villa.Id, booking.Nights, booking.CheckInDate))
            {
                TempData["error"] = "Rooms not available";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    villaId = booking.VillaId,
                    checkIndate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }

            _bookingService.CreateBooking(booking);

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
            _bookingService.UpdateStripePaymentID(booking.BookingId, session.Id, session.PaymentIntentId);
          

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {

            Booking booking =_bookingService.GetBookingById(bookingId);

            if(booking.Status == Constants.StatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(booking.StripeSessionId);

                if(session.PaymentStatus == "paid")
                {
                    //confirm is successful payment
                   _bookingService.UpdateStatus(bookingId, Constants.StatusApproved, 0);
                   _bookingService.UpdateStripePaymentID(booking.BookingId, session.Id, session.PaymentIntentId);
                   
                }
            }
            return View(bookingId);
        }

        [Authorize]
        [HttpPost]
        public IActionResult GenerateInvoice(int bookingId, string downloadType)
        {
            string basepath = _webHostEnvironment.WebRootPath;

            WordDocument  document = new WordDocument();

            //Loading the templalte
            string dataPath = basepath + @"/exports/BookingDetails.docx";

            using FileStream filestream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            document.Open(filestream, FormatType.Automatic);

            //Updating the template
            Booking booking = _bookingService.GetBookingById(bookingId);


            TextSelection textSelection = document.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Name;

            textSelection = document.Find("XX_BOOKING_NUMBER", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING ID - "+ booking.BookingId;


            textSelection = document.Find("XX_BOOKING_DATE", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "BOOKING DATE - " + booking.BookingDate.ToShortDateString();

            textSelection = document.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Email;

            textSelection = document.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.PaymentDate.ToShortDateString();

            textSelection = document.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckInDate.ToShortDateString();

            textSelection = document.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckOutDate.ToShortDateString();

            textSelection = document.Find("xx_booking_total", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.TotalCost.ToString("c");


            WTable table = new(document);
            table.TableFormat.Borders.LineWidth = 1f;
            table.TableFormat.Borders.Color = Color.Black;
            table.TableFormat.Paddings.Top = 7f;
            table.TableFormat.Paddings.Bottom = 7f;
            table.TableFormat.Borders.Horizontal.LineWidth = 7f;


            table.ResetCells(2, 4);

            WTableRow row0 = table.Rows[0];
            row0.Cells[0].AddParagraph().AppendText("NIGHTS");
            row0.Cells[0].Width = 80;

            row0.Cells[1].AddParagraph().AppendText("VILLA");
            row0.Cells[1].Width = 80;

            row0.Cells[2].AddParagraph().AppendText("PRICE");
            row0.Cells[2].Width = 80;

            row0.Cells[3].AddParagraph().AppendText("TOTAL");
            row0.Cells[3].Width = 80;

            WTableRow row1 = table.Rows[1];
            row1.Cells[0].AddParagraph().AppendText(booking.Nights.ToString());
            row1.Cells[0].Width = 80;
            row1.Cells[1].AddParagraph().AppendText(booking.Villa.Name + "-" + booking);
            row1.Cells[1].Width = 80;
            row1.Cells[2].AddParagraph().AppendText((booking.TotalCost/booking.Nights).ToString("c"));
            row1.Cells[2].Width = 80;
            row1.Cells[3].AddParagraph().AppendText(booking.TotalCost.ToString());
            row1.Cells[3].Width = 80;

            TextBodyPart bodyPart = new TextBodyPart(document);
            bodyPart.BodyItems.Add(table);

            document.Replace("<ADDTABLEHERE>", bodyPart, false, false);


            using DocIORenderer renderer = new();
            MemoryStream stream = new MemoryStream();

            if (downloadType == "word")
            {
                document.Save(stream, FormatType.Docx);
                stream.Position = 0;

                return File(stream, "application/docx", "BookingDetails.docx");
            }
            else
            {
                PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                pdfDocument.Save(stream);
                stream.Position = 0;

                return File(stream, "application/pdf", "BookingDetails.pdf");
            }

        }

        #region API CALLS
        [HttpGet]
        [Authorize(Roles = Constants.Role_Admin + "," + Constants.Role_Customer)]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> objBookings;
            string userId = "";
            if (string.IsNullOrEmpty(status))
            {
                status = "";
            }
            if (User.IsInRole(Constants.Role_Admin))
            {
                
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            }
            objBookings = _bookingService.GetAllBooking(userId, status);

          
            return Json(new { data = objBookings });    
        }



        #endregion
    }
}
