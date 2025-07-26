using BlueLagoon.Application.Common.Interfaces;
using BlueLagoon.Application.Services.Interface;
using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Application.Services.Implementation
{
    public class VillaSuiteService : IVillaSuiteService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VillaSuiteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CheckVillaSuiteExits(int id)
        {
            return _unitOfWork.VillaSuite.Any(u => u.VillaSuitId == id);
        }

        public void CreateVillaSuite(VillaSuite villaSuite)
        {
            _unitOfWork.VillaSuite.Add(villaSuite);
            _unitOfWork.Save();
        }

        public bool DeleteVillaSuite(int id)
        {
            try
            {
                VillaSuite villaSuite = _unitOfWork.VillaSuite.Get(c => c.VillaSuitId == id);

                if (villaSuite is not null)
                {
                    _unitOfWork.VillaSuite.Delete(villaSuite);
                    _unitOfWork.Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        public IEnumerable<VillaSuite> GetAllVillaSuites()
        {
            return _unitOfWork.VillaSuite.GetAll(includeProperties: "Villa");
        }

        public VillaSuite GetVillaSuiteById(int id)
        {
            return _unitOfWork.VillaSuite.Get(u => u.VillaSuitId == id, includeProperties: "Villa");

        }

        public void UpdateVillaSuite(VillaSuite villaSuite)
        {
                _unitOfWork.VillaSuite.Update(villaSuite);
                _unitOfWork.Save();
         
        }
    }
}
