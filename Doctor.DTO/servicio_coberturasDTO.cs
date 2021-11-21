using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class servicio_coberturasDTO 
    {
        public int ID { get; set; }
        public string descrip { get; set; }
        public List<coberturaDTO> coberturas { get; set; }

        
    }
}
