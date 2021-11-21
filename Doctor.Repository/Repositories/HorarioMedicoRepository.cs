using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class HorarioMedicoRepository : IHorarioMedicoRepository
    {
        private readonly MyDbContext _db;
        public HorarioMedicoRepository(MyDbContext db)
        {
            _db = db;

        }
        public horarioMedicoDTO getWorkDaySchedule(DayOfWeek dayOfWeek, int medicoID)
        {
            IQueryable<horarioMedicoDTO> hoursQr = null;

            if (dayOfWeek == DayOfWeek.Monday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.monday_from,
                               _until = h.monday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Tuesday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.tuesday_from,
                               _until = h.tuesday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Wednesday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.wednesday_from,
                               _until = h.wednesday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Thursday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.thursday_from,
                               _until = h.thursday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Friday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.friday_from,
                               _until = h.friday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Saturday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.saturday_from,
                               _until = h.saturday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Sunday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarioMedicoDTO
                           {
                               _from = h.sunday_from,
                               _until = h.sunday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            var workDaySchedule = hoursQr.FirstOrDefault();

            if (workDaySchedule == null)
                return null;

            if (workDaySchedule._from == null || workDaySchedule._until == null)
                return null;

            return workDaySchedule;
        }

        public async Task<List<DateTime>> getAvailableDayListAsync(int medicoID)
        {

            List<DateTime> holyDayslst = await _db.dias_feriados.Select(x => x.fecha).ToListAsync();
            var availableDaylst = new List<DateTime>();

            int maxDays = 30;

            List<DateTime> timelst = new List<DateTime>();
            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < maxDays; i++)
            {

                timelst = getAvailableHoursTurnDic(currentDate, medicoID)?.Select(x => x.Key).ToList(); ;


                if (timelst != null)
                {
                    //Store this date if it's available.
                    if (timelst.Except(holyDayslst).Count() > 0)
                        availableDaylst.Add(timelst.First().Date);
                }

                currentDate = currentDate.AddDays(1);
            }
            return availableDaylst.Except(holyDayslst).ToList();

        }


        public Dictionary<DateTime, int> getAvailableHoursTurnDic(DateTime fecha_hora, int medicoID)
        {

            horarioMedicoDTO doctorWorkSchedule = getWorkDaySchedule(fecha_hora.DayOfWeek, medicoID);
            if (doctorWorkSchedule == null)
                return null;

            int appointmentDuration = doctorWorkSchedule.tiempo_cita.Minutes;
            List<DateTime> reservedTimelst = getReservedHoursList(medicoID, fecha_hora);
            DateTime startTime = fecha_hora.Date.Add(doctorWorkSchedule._from.Value);
            DateTime endTime = fecha_hora.Date.Add(doctorWorkSchedule._until.Value);
            DateTime startFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_from.Value);
            DateTime endFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_until.Value);
            Dictionary<DateTime, int> availableTimeDic = new Dictionary<DateTime, int>();

            int nTurn = 0;

            while (startTime < endTime)
            {
                var intime = startTime.CompareTo(DateTime.Now);// compare today's date in order to not allow lower date than today 
                if ((startTime.TimeOfDay.CompareTo(startFreeTime.TimeOfDay) < 0 || startTime.TimeOfDay.CompareTo(endFreeTime.TimeOfDay) > 0) && intime > 0)
                {
                    nTurn++;
                    availableTimeDic.Add(startTime, nTurn);

                }
                startTime = startTime.AddMinutes(appointmentDuration);
            }

            //let's get rid of reserved hours
            if (reservedTimelst != null)
            {
                availableTimeDic = availableTimeDic
                .Where(kvp => !reservedTimelst.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return availableTimeDic;
        }
        public async Task<horarios_medicos_reservados> getReservedHourAsync(int? medicoID, DateTime fecha_hora)
        {
            horarios_medicos_reservados r = await _db.horarios_medicos_reservados.FirstOrDefaultAsync(h => h.fecha_hora == fecha_hora && h.medicosID == medicoID);

            return r;
        }


        public List<DateTime> getReservedHoursList(int? medicoID, DateTime fecha_hora)
        {

            List<DateTime> r = _db.horarios_medicos_reservados
                .Where(rt => rt.medicosID == medicoID && rt.fecha_hora.Date == fecha_hora.Date)
                .Select(rt => rt.fecha_hora).ToList();

            return r;
        }



        public bool isAvailableDateHour(DateTime fecha_hora, int medicoID)
        {

            try
            {

                var availableTimelst = getAvailableHoursTurnDic(fecha_hora, medicoID)?.Select(x => x.Key).ToList(); 

                if (availableTimelst == null || availableTimelst.Contains(fecha_hora))
                    throw new ArgumentException("Este doctor(a) no labora el día escogido.");

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

      
    }
}
