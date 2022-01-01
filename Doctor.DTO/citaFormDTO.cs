using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{

    public class citaFormDTO
    {
        public Medico_Info medico { get; set; }
        public List<servicio_coberturasDTO> servicios { get; set; }
        public List<DateTime> diasLaborables { get; set; }
    }

    public class Medico_Info
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
    }
}
