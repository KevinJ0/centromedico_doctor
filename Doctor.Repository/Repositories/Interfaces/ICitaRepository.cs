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
        public citas get(int citaId, int medicoId);
        public Task<List<citaDTO>> getCitasListAsync(int medicoId, DateTime? inicio = null, DateTime? fin = null, bool? estado = null, int? servicioId = null, int? seguroId = null);
        public List<citaDTO> getCitasListByCv(string codVerificacion);
        public bool Exist(medicos medico, string doc_identidad);
        public void Add(citas entity);
        public void Update(citas entity);
        public void Remove(citas entity);
        int getCantCitasPendientes(DateTime fecha_hora_cita, int medicosID);
        DateTime? getFirstTurnByDate(DateTime fecha_hora, int medicoID);
        DateTime? getLastTurnByDate(DateTime fecha_hora, int medicoID);



    }
}
