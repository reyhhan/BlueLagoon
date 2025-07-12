﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IVillaRepository Villa { get; }
        IVillaSuiteRepository VillaSuite { get; }
        IAmenityRepository Amenity { get; } 
        IBookingRepository Booking { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
