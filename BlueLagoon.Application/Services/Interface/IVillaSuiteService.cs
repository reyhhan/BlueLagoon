using BlueLagoon.Domain.Entities;

namespace BlueLagoon.Application.Services.Interface
{
    public interface IVillaSuiteService
    {
        IEnumerable<VillaSuite> GetAllVillaSuites();
        VillaSuite GetVillaSuiteById(int id);
        void CreateVillaSuite(VillaSuite villa);
        void UpdateVillaSuite(VillaSuite villa);
        bool DeleteVillaSuite(int id);
        bool CheckVillaSuiteExits(int id);
    }
}
