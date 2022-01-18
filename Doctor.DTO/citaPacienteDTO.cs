using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{

    public class citaPacienteDTO
    {

        public int? ID { get; set; }
        public int medicosID { get; set; }
        [Required]
        public string doc_identidad { get; set; }
        //public string medico_nombre { get; set; }
        //public string medico_apellido { get; set; }
        public int? serviciosID { get; set; }
        public int? pacientesID { get; set; }
        [Required]
        public string paciente_nombre { get; set; }
        [Required]
        public string? paciente_apellido { get; set; }
        public string paciente_nombre_tutor { get; set; }
        public string? paciente_apellido_tutor { get; set; }
        [Required]
        public DateTime fecha_nacimiento { get; set; }
        [StringLength(1)]
        [Required]
        public string sexo { get; set; }
        public string contacto { get; set; }
        public int? edad { get; set; }
        public bool? menor_un_año { get; set; }
        [Column(TypeName = "text")]
        public string nota { get; set; }
        public bool contacto_whatsapp { get; set; }
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime fecha_hora { get; set; }
        public int? segurosID { get; set; }
        public int turno { get; set; }
    }

}
