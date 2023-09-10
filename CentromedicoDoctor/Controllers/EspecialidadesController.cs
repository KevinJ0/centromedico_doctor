

using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CentromedicoDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadRepository _especRepo;
        private readonly IEspecialidadService _especSvc;


        public EspecialidadesController(
            IEspecialidadService especSvc, 
            IEspecialidadRepository especRepo)
        {
            _especSvc = especSvc;
            _especRepo = especRepo;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpGet("[action]")]
        public ActionResult<List<especialidadDTO>> get()
        {
            try
            {

                List<especialidadDTO> result = _especRepo.get();

                if (!result.Any())
                    return new NoContentResult();

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }


        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpGet("[action]")]
        public ActionResult<List<especialidadDTO>> getAll()
        {
            try
            {

                List<especialidadDTO> result = _especRepo.getAll();

                if (!result.Any())
                    return new NoContentResult();

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpDelete("[action]")]
        public ActionResult delete(int id_espec)
        {
            try
            {

                _especSvc.deleteEspecMedico(id_espec);

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPost("[action]")]
        public ActionResult add([FromBody] int id_espec)
        {
            try
            {

                _especSvc.addEspecMedico(id_espec);

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }


        }

    }
}

