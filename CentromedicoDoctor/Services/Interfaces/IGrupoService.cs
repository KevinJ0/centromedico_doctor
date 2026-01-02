using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IGrupoService
    {
        Task<Dictionary<string, string>> get(int medicoID);
        Task<grupo_doctor_secretaria> getGrupoTurnoAsync(int medicoID);
    }
}
