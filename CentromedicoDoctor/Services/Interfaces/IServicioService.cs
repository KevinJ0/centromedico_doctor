using Doctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IServicioService
    {
        public Task<List<servicio_coberturasDTO>> getServicio_coberturaByDoctorIdAsync(int medicoID);
        public Task<List<serviciosDTO>> getAllAsync(int medicoID);

    }
}
