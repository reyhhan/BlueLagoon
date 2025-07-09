using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;

namespace BlueLagoon.Infrastructure.Repository
{
    public class AmenityRepository : Repository<Amenity>, IAmenityRepository
    {
        private readonly ApplicationDbContext _db;
        public AmenityRepository(ApplicationDbContext db)  : base(db)
        {
            _db = db;
        }

        public void Update(Amenity amenity)
        {
            _db.Amenities.Update(amenity);
        }
    }
}
