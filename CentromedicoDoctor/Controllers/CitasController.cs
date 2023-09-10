
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Doctor.DTO;
using CentromedicoDoctor.Services.Interfaces;
using CentromedicoDoctor.Exceptions;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {

        private readonly ICitaService _citaSvc;
        public CitasController(ICitaService citaSvc)
        {
            _citaSvc = citaSvc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/SaveCita
        ///      
        /// </remarks>
        /// <returns></returns>
        /// <response code="500"></response>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpPost("[action]")]
        public async Task<ActionResult> entryCita(citaEntryDTO formdata)
        {

            try
            {
                var result = await _citaSvc.entryCita(formdata);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Devuelve las citas vículadas con el usuario registrado. Estas pueden ser las citas que son para esta misma persona
        /// ó para un menor de edad y este.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/getCitasList
        ///      
        /// </remarks>
        /// <returns>List of citaDTO</returns>
        /// <response code="204">No hay ninguna cita vículada con este usuario.</response>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<citaDTO>>> getCitasListAsync(int medicoId, DateTime? inicio = null,
                                                                         DateTime? fin = null, bool? estado = null,
                                                                         int? servicioId = null, int? seguroId = null)
            {

            try
            {
                var result = await _citaSvc.getCitasListAsync(medicoId, inicio, fin, estado, servicioId, seguroId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpGet]
        public ActionResult<citaDTO> getCita(int citaId, int? medicoId)
        {
            try
            {
                var result = _citaSvc.get(citaId, medicoId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpGet("[action]")]
        public async Task<ActionResult<citaFormDTO>> getCitaFormAsync(int citaId, int medicoId)
        {
            try
            {

                var result = await _citaSvc.getFormCitaAsync(citaId, medicoId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpGet("[action]")]
        public async Task<ActionResult<citaPacienteDTO>> getCitaPacienteAsync(int citaId, int? medicoId)
        {
            try
            {

                var result = await _citaSvc.getCitaPatienteAsync(citaId, medicoId);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpPut("{citaID:int}")]
        public async Task<ActionResult<bool>> updateCitaAsync(int citaID, citaPacienteDTO formdata)
        {
            try
            {

                bool result = await _citaSvc.updateCitaAsync(citaID, formdata);
                return result;
            }
            catch (Exception)
            {
                throw;

            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpPut("[action]/{citaID:int}")]
        public async Task<ActionResult> updateDateTimeAsync(int citaID, citaDateTimeDTO formdata)
        {
            try
            {
                _citaSvc.updateDateTimeAsync(citaID, formdata).Wait();

                return Ok();

            }
            catch (Exception)
            {
                throw;

            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
        [HttpDelete("{citaID:int}/{medicoID:int}")]
        public async Task<ActionResult> deleteCitaAsync(int citaID, int medicoID)
        {
            try
            {
                _citaSvc.deleteCita(citaID, medicoID);
                return Ok();
            }
            catch (Exception)
            {
                throw;

            }
        }
    }

}
