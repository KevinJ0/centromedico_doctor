using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using System.Linq;

namespace Doctor.Repository.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly MyDbContext _db;
        public MedicoRepository(MyDbContext db)
        {
            _db = db;
        }

        public medicos get(MyIdentityUser user)
        {
            medicos _medico = _db.medicos.FirstOrDefault(x=> x.MyIdentityUsers == user);
            return _medico;
        }
    }
}
