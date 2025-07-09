using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Domain.Entities;
using BlueLagoon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Infrastructure.Repository
{
    public class VillaRepository : Repository<Villa> , IVillaRepository
    {
        private readonly ApplicationDbContext _context;
        public VillaRepository(ApplicationDbContext context ) : base( context )
        {
            _context = context;
        }

        public void Update(Villa villa)
        {
           _context.Update(villa);
        }
    }
}
