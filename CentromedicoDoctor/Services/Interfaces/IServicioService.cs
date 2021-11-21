using Doctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IServicioService
    {
        public Task<List<servicio_coberturasDTO>> getAllByDoctorIdAsync(int medicoID);
    }
}
