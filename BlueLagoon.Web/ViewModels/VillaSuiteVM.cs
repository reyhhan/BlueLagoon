using BlueLagoon.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlueLagoon.Web.ViewModels
{
    public class VillaSuiteVM
    {
        public VillaSuite? VillaSuite { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }
    }
}
