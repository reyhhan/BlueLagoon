using BlueLagoon.Domain.Entities;

namespace BlueLagoon.Application.Services.Interface
{
    public interface IAmenityService
    {
        IEnumerable<Amenity> GetAllAmenities();
        Amenity GetAmenityById(int id);
        void CreateAmenity(Amenity villa);
        void UpdateAmenity(Amenity villa);
        bool DeleteAmenity(Amenity villa);

    }
}
