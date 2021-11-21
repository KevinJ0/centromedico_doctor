using Doctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface ICitaService
    {
        Task<List<citaDTO>> getCitasListAsync();
    }
}
