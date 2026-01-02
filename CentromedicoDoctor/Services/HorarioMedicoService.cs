using Centromedico.Database.Context;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services
{
    public class HorarioMedicoService : IHorarioMedicoService
    {
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;
        private readonly IMedicoRepository _medicoRepo;

        private readonly MyDbContext _db;
        public HorarioMedicoService(
            IMedicoRepository medicoRepo,
            IHorarioMedicoRepository horarioMedicoRepo,
            MyDbContext db)
        {
            _medicoRepo = medicoRepo;
            _db = db;
            _horarioMedicoRepo = horarioMedicoRepo;

        }


        //Lista de horas disponibles
        public Dictionary<DateTime, int> getHoursList(DateTime fecha_hora, int medicoId)
        {
            try
            {

                //get the  appointment list schedule of this doctor
                int medicoID = _medicoRepo.validMedicoIdAsync(medicoId).Result;

                var dateTimeDic = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID);

                if (dateTimeDic == null)
                    throw new EntityNotFoundException("Este médico no labora el día escogido: " + fecha_hora.Date.ToString("dd/MM/yyyy"));
                else if (!dateTimeDic.Any())
                    throw new BadHttpRequestException("El médico no cuenta con días hábiles en la fecha escogida: " + fecha_hora.Date.ToString("dd/MM/yyyy"));

                return dateTimeDic;


            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
