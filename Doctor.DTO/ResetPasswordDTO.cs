using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class ResetPasswordDTO
    {
        [Required(ErrorMessage = "Contraseña es requerida")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "La confirmación es requerida")]
        public string? ConfirmPassword { get; set; }
    }
}
