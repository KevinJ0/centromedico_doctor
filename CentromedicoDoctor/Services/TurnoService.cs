using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services
{
    public class TurnoService : ITurnoService
    {

        private readonly MyDbContext _db;
        private readonly ITurnoRepository _turnoRepo;
        private readonly ICitaRepository _citaRepo;
        private readonly IMapper _mapper;

        public TurnoService(
            MyDbContext db,
            ITurnoRepository turnoRepo,
            IMapper mapper,
            ICitaRepository citaRepo)
        {
            _db = db;
            _turnoRepo = turnoRepo;
            _mapper = mapper;
            _citaRepo = citaRepo;
        }
        public async Task<turnoDTO> getTurnoPaciente(DateTime fecha_hora_cita, int medicosID) {

            var turno = await _turnoRepo.getAsync(medicosID);

            var turnoDto =  _mapper.Map<turnoDTO>(turno);

            turnoDto.cant_pacientes_adelante = _citaRepo.getCantCitasPendientes(fecha_hora_cita, medicosID); 
            turnoDto.primera_entrada = _citaRepo.getFirstTurnByDate(fecha_hora_cita, medicosID); 
            turnoDto.ultima_entrada = _citaRepo.getLastTurnByDate(fecha_hora_cita, medicosID); ;

            return turnoDto;
        }

        public async Task SaveTurnoAsync(int turno, int medicosID)
        {

            var res = await _turnoRepo.getAsync(medicosID);

            if (res == null)
            {

                var newTurno = new turnos {  
                    medicosID = medicosID, 
                    turno_actual = turno, 
                    turno_atendido = 1,
                    fecha = DateTime.Now
                };

                await _turnoRepo.addAsync(newTurno);
            }
            else
            {
                //última fecha no es de hoy, pues lo actualizo
                if (res.fecha.Date < DateTime.Now.Date)
                {
                    res.turno_atendido = 0;
                }

                res.turno_atendido += 1; // last-turn
                res.fecha = DateTime.Now;
                res.turno_actual = turno;

                await _turnoRepo.updateAsync(res);
            }

            await _db.SaveChangesAsync();
        }


    }
}

