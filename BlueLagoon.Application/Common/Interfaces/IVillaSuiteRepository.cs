using BlueLagoon.Domain.Entities;

namespace BlueLagoon.Application.Common.Interfaces
{
    public interface IVillaSuiteRepository : IRepository<VillaSuite>
    {
        void Update(VillaSuite villaSuite);
    }
}
