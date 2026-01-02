using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Centromedico.Database.DbModels;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor, Patient")]
    [ApiController]
    public class GruposController : ControllerBase
    {
        private readonly IGrupoService _grupoSvc;

        public GruposController(IGrupoService grupoSvc)
        {
            _grupoSvc = grupoSvc;
        }


        [HttpGet]
        public async Task<ActionResult<Dictionary<string, string>>> getAsync(int medicoID)
        {
            try
            {

                Dictionary<string, string> result = await _grupoSvc.get(medicoID);
                return result;

            }
            catch (Exception)
            {

                throw;

            }
        }
        [HttpGet("[action]")]
        public async Task<grupo_doctor_secretaria> getGrupoTurnoAsync(int medicoID)
        {
            try
            {

                var result = await _grupoSvc.getGrupoTurnoAsync(medicoID);
                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

        

    }
}
