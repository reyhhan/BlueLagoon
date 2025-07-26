using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Application.Utilities;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace BlueLagoon.Application.Services.Implementation
{
    public class VillaService : IVillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public void CreateVilla(Villa villa)
        {
            if (villa.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");
                using var filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create);

                villa.Image.CopyTo(filestream);
                villa.ImageUrl = @"\images\VillaImages\" + fileName;

            }
            else
            {
                villa.ImageUrl = "https://placehold.co/600x400";
            }

            _unitOfWork.Villa.Add(villa);
            _unitOfWork.Save();
        }

        public bool DeleteVilla(int id)
        {
            Villa? villa = _unitOfWork.Villa.Get(u => u.Id == id);
            if (villa is not null)
            {
                if (!string.IsNullOrEmpty(villa.ImageUrl))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                _unitOfWork.Villa.Delete(villa);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public IEnumerable<Villa> GetAllVillas()
        {
            return _unitOfWork.Villa.GetAll(includeProperties : "VillaAmenities");
        }

        public Villa GetVillaById(int id)
        {
            return _unitOfWork.Villa.Get(u=>u.Id == id, includeProperties: "VillaAmenities");
        }

        public IEnumerable<Villa> GetVillasAvailabilityByDate(int nights, DateOnly checkInDate)
        {
            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenities");
            var villaSuites = _unitOfWork.VillaSuite.GetAll().ToList();

            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == Constants.StatusApproved || u.Status == Constants.StatusCheckedIn).ToList();
            foreach (var villa in villaList)
            {
                int roomsAvaialble = Constants.VillaRoomsAvailable_Count(villa.Id, villaSuites, checkInDate, nights, bookedVillas);

                villa.IsAvailable = roomsAvaialble > 0 ? true : false;
            }

            return villaList;
        }

        public bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate)
        {
            var villaSuites = _unitOfWork.VillaSuite.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == Constants.StatusApproved || u.Status == Constants.StatusCheckedIn).ToList();

            int roomsAvaialble = Constants.VillaRoomsAvailable_Count(villaId, villaSuites, checkInDate, nights, bookedVillas);

            return roomsAvaialble > 0;
        }

        public void UpdateVilla(Villa villa)
        {
            if (villa.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImages");


                if (!string.IsNullOrEmpty(villa.ImageUrl))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                using var filestream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                villa.Image.CopyTo(filestream);
                villa.ImageUrl = @"\images\VillaImages\" + fileName;

            }

            _unitOfWork.Villa.Update(villa);
            _unitOfWork.Save();
        }
    }
}
