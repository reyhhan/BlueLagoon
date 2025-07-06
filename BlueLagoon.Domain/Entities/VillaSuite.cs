using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueLagoon.Domain.Entities
{
    public class VillaSuite
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Suite Id")]
        [Range(1, 1000)]
        public int VillaSuitId {  get; set; }

        [ForeignKey("Villa")]
        public int VillaId {  get; set; }
        [ValidateNever]
        public Villa Villa { get; set; }
        public string? SpecialDetails {  get; set; }
    }
}
