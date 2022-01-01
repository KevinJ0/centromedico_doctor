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

    public class citaUserDTO
    {

        int _edad;
        public UserInfo userInfo { get; set; }
        public int ID { get; set; }
        public int? medicosID { get; set; }

        public string medico_nombre { get; set; }
        public string medico_apellido { get; set; }

#pragma warning restore CS8632 
        public int? serviciosID { get; set; }

        [StringLength(8)]
#pragma warning disable CS8632 
        public int? pacientesID { get; set; }
        public string paciente_nombre { get; set; }
#pragma warning disable CS8632 
        public string? paciente_apellido { get; set; }
#pragma warning restore CS8632 
        public string paciente_nombre_tutor { get; set; }
#pragma warning disable CS8632 
        public string? paciente_apellido_tutor { get; set; }
#pragma warning restore CS8632
        public int edad { get; set; }
        public bool menor_un_año { get; set; }
        [Column(TypeName = "text")]
        public string nota { get; set; }
        [StringLength(10)]
        public bool? contacto_whatsapp { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime fecha_hora { get; set; }
        public int segurosID { get; set; }
    }

}
