using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class citaResultDTO
    {

        [StringLength(8)]
        public string cod_verificacion { get; set; }    
        public string servicio { get; set; }
       // public string especialidad { get; set; }
        public int consultorio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime fecha_hora { get; set; }

        public string medico_nombre_apellido { get; set; }
       // public string especialidad { get; set; }
        public string seguro { get; set; }
        [Column(TypeName = "money")]
        public decimal pago { get; set; }
        [Column(TypeName = "money")]
        public decimal? cobertura { get; set; }
        [Column(TypeName = "money")]
        public decimal diferencia { get; set; }

        public string? doc_identidad { get; set; }
        public string paciente_nombre_apellido { get; set; }

        public string doc_identidad_tutor { get; set; } 
        public string tutor_nombre_apellido { get; set; }

        [StringLength(10)]
        public string contacto { get; set; }
        public string correo { get; set; }
        public int turno { get; set; }

       
    }
}
