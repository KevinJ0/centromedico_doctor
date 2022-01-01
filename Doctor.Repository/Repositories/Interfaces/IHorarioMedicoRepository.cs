using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IHorarioMedicoRepository
    {
        public Dictionary<DateTime, int> getAvailableHoursTurnDic(DateTime fecha_hora, int medicoID);
        public Task<List<DateTime>> getAvailableDayListAsync(int medicoID);
        public horarioMedicoDTO getWorkDaySchedule(DayOfWeek dayOfWeek, int medicoID);
        Task<horarios_medicos_reservados> getReservedHourAsync(int? medicoID, DateTime fecha_hora);
        bool isAvailableDateHour(DateTime fecha_hora, int medicosID);
    }
}
