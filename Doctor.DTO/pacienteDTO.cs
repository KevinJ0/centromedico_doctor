using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class PacienteDto
    {
        public int ID { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string sexo { get; set; }
        public DateTime fecha_nacimiento { get; set; }
        public string doc_identidad { get; set; }
        public string nombre_tutor { get; set; }
        public string apellido_tutor { get; set; }
        public bool extranjero { get; set; }
        public string contacto { get; set; }
        public string doc_identidad_tutor { get; set; }
        public int? edad { get; set; }
        public bool confirm_doc_identidad { get; set; }
        public bool menor_un_año { get; set; }
        public string correo { get; set; }
    }

}
