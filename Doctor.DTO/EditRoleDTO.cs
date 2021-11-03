using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.DTO
{
    public class EditRoleDTO 
    {

       public  EditRoleDTO() {
          Users = new List<string>();
        }

        public string Id { get; set; }
        [Required]
        [DisplayName("Nombre")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}
