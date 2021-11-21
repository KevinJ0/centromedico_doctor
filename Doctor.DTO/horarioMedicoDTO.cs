using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class horarioMedicoDTO
    {
        [Column(TypeName = "time(4)")]
        public TimeSpan tiempo_cita { get; set; }
        [Column(TypeName = "time(4)")]
        public TimeSpan? _from { get; set; }
        [Column(TypeName = "time(4)")]
        public TimeSpan? _until { get; set; }
        [Column(TypeName = "time(4)")]
        public TimeSpan? free_time_from { get; set; }
        [Column(TypeName = "time(4)")]
        public TimeSpan? free_time_until { get; set; }
    }
}
