using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using BlueLagoon.Infrastructure.Repository;

namespace BlueLagoon.Infrastructure
{
    public class VillaSuiteRepository : Repository<VillaSuite>, IVillaSuiteRepository
    {
        private readonly ApplicationDbContext _db;
        public VillaSuiteRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(VillaSuite villaSuite)
        {
            _db.VillaSuites.Update(villaSuite);
        }
    }
}
