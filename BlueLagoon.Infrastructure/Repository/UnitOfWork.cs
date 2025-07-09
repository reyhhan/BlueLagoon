using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Infrastructure.Data;

namespace BlueLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IVillaRepository Villa { get; private set; }
        public IVillaSuiteRepository VillaSuite { get; private set; }
        public IAmenityRepository Amenity { get; private set; } 

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Villa = new VillaRepository(_db);
            VillaSuite = new VillaSuiteRepository(_db);
            Amenity = new AmenityRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }

}
