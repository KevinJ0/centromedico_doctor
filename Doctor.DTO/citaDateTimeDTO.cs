using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor.DTO
{

    public class citaDateTimeDTO
    {

        public int? medicosID { get; set; }
      
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime fecha_hora { get; set; }
    }

}
