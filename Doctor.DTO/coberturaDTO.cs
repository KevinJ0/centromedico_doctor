using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class coberturaDTO
    {

        public int segurosID { get; set; }
        public string descrip { get; set; }

        public byte porciento { get; set; }
        [Column(TypeName = "money")]
        public decimal pago { get; set; }
        [Column(TypeName = "money")]
        public decimal cobertura { get => pago * Decimal.Divide((porciento), 100); }
        [Column(TypeName = "money")]
        public decimal diferencia { get => pago - cobertura; }
    }
}
