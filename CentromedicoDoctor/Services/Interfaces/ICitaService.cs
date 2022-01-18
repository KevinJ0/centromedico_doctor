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
        Task<bool> entryCita(citaEntryDTO formdata);
        Task<citaFormDTO> getFormCitaAsync(int citaId, int medicoId);
        Task<citaPacienteDTO> getCitaPatienteAsync(int citaId, int? medicoId);
        Task<bool> updateCitaAsync(int citaID, citaPacienteDTO formdata);
    }
}
