using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor.DTO
{
    public class citaCreateDTO
    {
        private string _sexo;


        [Required]
        public int medicosID { get; set; }
        [Required]
        public int? serviciosID { get; set; }
        [Column(TypeName = "text")]
        public string nota { get; set; }
        [StringLength(10)]
        public string contacto { get; set; }
        public bool? contacto_whatsapp { get; set; }
        [Column(TypeName = "datetime")]
        [Required]
        public DateTime fecha_hora { get; set; }
        public int? segurosID { get; set; }
        public string? cod_verificacionID { get; set; }


        //Patient
        [Required]
        public int appointment_type { get; set; }
        [Required]
        [StringLength(40)]
        public string nombre { get; set; }
        [StringLength(50)]
        public string apellido { get; set; }
        [Required]
        [StringLength(1)]
        public string sexo {
            get => _sexo;

            set {
                if(value != "m" || value != "f")
                _sexo = value;
            } 
        }
        [Column(TypeName = "date")]
        public DateTime fecha_nacimiento { get; set; }

        public bool extranjero { get; set; }
        [StringLength(40)]
        public string correo { get; set; }
        [StringLength(15)]
        public int? edad { set; get; }
        public bool menor_un_año { get; set; }

        //user incoming data
        public UserInfo? userinfo;

    }
}
