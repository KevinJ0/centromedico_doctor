using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Doctor.DTO;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary, Doctor, Patient")]
    [ApiController]
    public class TurnosController : ControllerBase
    {
        private readonly ITurnoRepository _turnoRepo;
        private readonly ITurnoService _turnoSvc;

        public TurnosController(ITurnoRepository turnoRepo, ITurnoService turnoSvc)
        {
            _turnoRepo = turnoRepo;
            _turnoSvc = turnoSvc;
        }


        [HttpGet]
        public async Task<ActionResult<turnos>> getAsync(int medicoID)
        {
            try
            {

                turnos turno = await _turnoRepo.getAsync(medicoID);
                return turno;

            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<turnoDTO>> getTurnoPaciente(DateTime fecha_hora_cita, int medicosID)
        {
            try
            {

                turnoDTO turnoDto = await _turnoSvc.getTurnoPaciente(fecha_hora_cita, medicosID);
                return turnoDto;

            }
            catch (Exception)
            {

                throw;

            }
        }

        [HttpPost]
        public async Task<ActionResult> saveAsync(int turno, int medicosID)
        {
            try
            {
                await _turnoSvc.SaveTurnoAsync(turno, medicosID);
            }
            catch (Exception)
            {

                throw;
            }

            return Ok();
        }


    }
}
