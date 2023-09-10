using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CentromedicoDoctor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurosController : ControllerBase
    {
        private readonly ISeguroService _seguroSvc;


        public SegurosController(ISeguroService seguroSvc)
        {
            _seguroSvc = seguroSvc;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<segurosDTO>>> getSegurosAsync(int medicoID)
        {
            try
            {
                List<segurosDTO> result = await _seguroSvc.getAllByDoctorIdAsync(medicoID);

                if (!result.Any())
                    return new NoContentResult();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
         
    }
}
