using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable
namespace Doctor.DTO

{
    public partial class turnoDTO
    {
        public int medicosID { get; set; }
        public int turno_actual { get; set; }
        public int turno_atendido { get; set; }
        public int cant_pacientes_adelante { get; set; } = 0;
        public DateTime? fecha { get; set; }
        public DateTime? primera_entrada { get; set; }
        public DateTime? ultima_entrada { get; set; }

    }
}
