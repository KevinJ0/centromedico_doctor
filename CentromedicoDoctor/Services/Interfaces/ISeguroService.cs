using Centromedico.Database.DbModels;
using Doctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface ISeguroService
    {
        public Task<List<segurosDTO>> getAllByDoctorIdAsync(int medicoID);

    }
}
