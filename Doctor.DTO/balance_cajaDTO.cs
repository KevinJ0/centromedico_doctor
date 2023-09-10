using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Doctor.DTO
{
    [Index(nameof(secretariasID), Name = "IX_balance_caja_secretariasID")]
    public partial class balance_cajaDTO
    {
        [Key]
        public int? medicosID { get; set; }
        [Key]
        [Column(TypeName = "date")]
        public int secretariasID { get; set; }
        [Column(TypeName = "money")]
        public decimal balance_inicial { get; set; }
    }
}
