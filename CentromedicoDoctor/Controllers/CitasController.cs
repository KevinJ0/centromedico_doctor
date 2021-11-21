
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Doctor.DTO;
using CentromedicoDoctor.Services.Interfaces;


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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor,Secretary")]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<citaDTO>>> getCitasListAsync()
        {

            try
            {
                var result = await _citaSvc.getCitasListAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

    
        enum appointment : int
        {
            me = 0,
            other = 1,
        }
    }

}
