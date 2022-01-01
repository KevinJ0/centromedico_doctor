using Doctor.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface ICitaService
    {
        citaDTO get(int Id, int? medicoId);
        Task<List<citaDTO>> getCitasListAsync(int? medicoId);
        Task<bool> saveCita(citaEntryDTO formdata);
        Task<citaFormDTO> getFormCitaAsync(int citaId, int medicoId);
        Task<citaUserDTO> getCitaPatienteAsync(int citaId, int? medicoId);
    }
}
