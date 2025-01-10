using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IPacienteService
    {
        Task<List<PacienteDto>> getPacienteListAsync(int medicoId);
        Task<bool> saveUserInfoAsync(UserInfo formuser);
        Task<bool> updateAsync(UserInfo formuser);
    }
}
