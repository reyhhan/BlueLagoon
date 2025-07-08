using BlueLagoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Common.Interfaces
{
    public interface IVillaRepository : IRepository<Villa>
    {
        void Update(Villa villa);
        void Save();
    }
}
