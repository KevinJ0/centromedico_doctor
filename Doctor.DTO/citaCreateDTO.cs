using Centromedico.Database.DbModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor.DTO
{
    public class citaCreateDTO
    {


        [Required]
        public int medicosID { get; set; }
        [Required]
        public int? serviciosID { get; set; }
        [Column(TypeName = "text")]
        public string nota { get; set; }
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime fecha_hora { get; set; }
        public int? segurosID { get; set; }
        public string cod_verificacionID { get; set; }
        [Required]
        public int appointment_type { get; set; }
        public bool? contacto_whatsapp { get; set; }

        //
        //
        //
        //
        //Patient
        public pacientes paciente{ get; set; }

        //user incoming data
        //public UserInfo userinfo;

    }
}
