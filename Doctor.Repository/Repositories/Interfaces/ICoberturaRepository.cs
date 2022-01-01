using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface ICoberturaRepository
    {
        public Task<List<coberturaDTO>> getAllByDoctorIdAsync(int medicoID);
        public Task<coberturaMedicoDTO> getAsync(int medicosID, int? segurosID, int? serviciosID);
    }
}
