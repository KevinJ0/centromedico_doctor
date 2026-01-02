using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface ITurnoService
    {
        Task SaveTurnoAsync(int turno, int medicosID);
        Task<turnoDTO> getTurnoPaciente(DateTime fecha_hora_cita, int medicosID);

    }
}