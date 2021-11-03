using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class UserInfo
    {
        [StringLength(15)]
        [Required]
        public string doc_identidad { get; set; }
        [StringLength(50)]
        [Required]
        public string nombre { get; set; }
        [StringLength(50)]
        [Required]
        public string apellido { get; set; }
        [StringLength(1)]
        [Required]
        public string sexo { get; set; }
        [StringLength(15)]
        public string contacto { get; set; }
        [Required]
        public DateTime fecha_nacimiento { get; set; }
        public bool confirm_doc_identidad { get; set; }


    }
}
