using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class HorarioMedicoReservaRepository : IHorarioMedicoReservaRepository
    {

        private readonly MyDbContext _db;

        public HorarioMedicoReservaRepository(MyDbContext db)
        {
            _db = db;
        }
        public void Add(horarios_medicos_reservados entity) {
        
                _db.horarios_medicos_reservados.Add(entity);
        
        }
    }
}
