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


    }
}
