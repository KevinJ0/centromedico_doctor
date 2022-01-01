using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using System.Net;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor")]
    [ApiController]
    public class Horarios_MedicosController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly IHorarioMedicoService _horarioMedicoSvc;

        public Horarios_MedicosController(IHorarioMedicoService horarioMedicoSvc, MyDbContext db)
        {
            _horarioMedicoSvc = horarioMedicoSvc;
            _db = db;
        }


        /// <summary>
        /// Devuelve un diccionario datetime y init que representa los horarios disponibles de este médico , tomando como referencia 
        /// el día que se le suministra y el turno que le concierne a la fecha. Retorna null si el médico no labora ese día.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/ValidateDate?fecha_hora=2021-06-21T08:00:00&amp;medicoID=1
        ///      
        /// </remarks>
        /// <param name="fecha_hora"></param>
        /// <param name="medicoID"></param>
        /// <returns>List of DateTime</returns>
        /// <response code="204">No hay horarios disponibles para hacer consultas este día.</response>  
        /// <response code="400">El médico no labora este día.</response>  
        [HttpGet("[action]")]
            public ActionResult<Dictionary<DateTime, int>> getHoursList(DateTime fecha_hora, int medicoID)
            {
                try
                {
                    //get the  appointment list schedule of this doctor
                    var result = _horarioMedicoSvc.getHoursList(fecha_hora, medicoID);
                    return result;
                }
                catch (Exception)
                {

                    throw;
                }
            }

     
    }
}
