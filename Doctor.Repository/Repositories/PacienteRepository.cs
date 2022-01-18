using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly MyDbContext _db;

        public PacienteRepository(MyDbContext db)
        {
            _db = db;
        }

        public void Add(pacientes entity)
        {

            _db.pacientes.Add(entity);
        }


        public pacientes get(int pacienteId)
        {
            var r = _db.pacientes
                     .FirstOrDefault(p => p.ID == pacienteId); //this is for add another row with the same ID tutor.

            return r;
        }

        public async Task<string> getEmailAsync(int pacienteId)
        {
            var r = await _db.pacientes.Include("MyIdentityUsers")
                     .FirstOrDefaultAsync(x => x.ID == pacienteId); //this is for add another row with the same ID tutor.

            if (r.MyIdentityUsers != null)
                return r.MyIdentityUsers.Email;

            return null;



        }

        public void Update(pacientes entity)
        {
            _db.pacientes.Update(entity);
        }
    }
}
