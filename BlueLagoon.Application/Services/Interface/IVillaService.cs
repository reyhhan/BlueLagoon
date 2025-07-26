using BlueLagoon.Domain.Entities;


namespace BlueLagoon.Application.Services.Interface
{
    public interface IVillaService
    {
        IEnumerable<Villa> GetAllVillas();
        Villa GetVillaById(int id);
        void CreateVilla(Villa villa);
        void UpdateVilla(Villa villa);
        bool DeleteVilla(int id);

        bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate);
        IEnumerable<Villa> GetVillasAvailabilityByDate(int nights, DateOnly checkInDate);
    }
}
