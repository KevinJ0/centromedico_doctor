using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class citaDTO
    {
        int _edad;
        public int ID { get; set; }
        public int? medicosID { get; set; }

        public string medico_nombre { get; set; }
        public string medico_apellido { get; set; }

#pragma warning restore CS8632 
        public int? serviciosID { get; set; }
        public string servicio_descrip { get; set; }

        [StringLength(8)]
#pragma warning disable CS8632 
        public string? cod_verificacionID { get; set; }
#pragma warning restore CS8632 
        public int? pacientesID { get; set; }

        public string doc_identidad { get; set; }
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
        public string contacto { get; set; }
        public bool? contacto_whatsapp { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime fecha_hora { get; set; }
        public int segurosID { get; set; }

        public string seguro_descrip { get; set; }

        //public int secretariasID { get; set; }
        [Column(TypeName = "money")]
        public decimal diferencia { get; set; }
        [Column(TypeName = "money")]
        public decimal? cobertura { get; set; }
        [Column(TypeName = "money")]
        public decimal descuento { get; set; }
        [Column(TypeName = "money")]
        public decimal pago { get; set; }
        /*public int especialidadesID { get; set; }
          public string especialidad_descrip { get; set; }*/
        public int consultorio { get; set; }
        public int turno { get; set; }
        public bool estado{ get; set; }
        public TimeSpan appointmentDuration { get; set; }


    }
}
