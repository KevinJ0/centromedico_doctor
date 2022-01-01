using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface ICitaRepository
    {
        public  citas get(int Id, int medicoId);
        public Task<List<citaDTO>> getCitasListAsync(int medicoId);
        public List<citaDTO> getCitasListByCv(string codVerificacion);
        public bool Exist(medicos medico, MyIdentityUser user);
        public bool Exist(MyIdentityUser user);
        public void Add(citas entity);
        void saveCita(citas cita);
    }
}
