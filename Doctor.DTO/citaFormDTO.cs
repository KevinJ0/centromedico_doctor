using System;
using System.Collections.Generic;

namespace Doctor.DTO
{

    public class citaFormDTO
    {
        public medicoInfo medico { get; set; }
        public List<servicio_coberturasDTO> servicios { get; set; }
        public List<DateTime> diasLaborables { get; set; }
    }
     
    public class medicoInfo
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
    }
}
