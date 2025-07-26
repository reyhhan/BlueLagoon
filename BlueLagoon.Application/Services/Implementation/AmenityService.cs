using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void CreateAmenity(Amenity amenity)
        {
            _unitOfWork.Amenity.Add(amenity);
            _unitOfWork.Save();
        }

        public bool DeleteAmenity(Amenity amenity)
        {
            try
            {       
                if (amenity is not null)
                {
                    _unitOfWork.Amenity.Delete(amenity);
                    _unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public IEnumerable<Amenity> GetAllAmenities()
        {
            return _unitOfWork.Amenity.GetAll(includeProperties: "Villa").ToList();
        }

        public Amenity GetAmenityById(int id)
        {
            return _unitOfWork.Amenity.Get(u=>u.Id== id, includeProperties: "Villa");
        }

        public void UpdateAmenity(Amenity amenity)
        {
           _unitOfWork.Amenity.Update(amenity);
           _unitOfWork.Save();
        }
    }
}
