using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class citaEntryDTO
    {

        public int ID { get; set; }
        public int? medicoID { get; set; } // just for secretary usage
        public string observacion { get; set; }
        [Column(TypeName = "money")]
        public decimal? descuento { get; set; }


    }
}
