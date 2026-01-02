using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Doctor.Repository.Repositories
{
    public class CitaRepository : ICitaRepository
    {

        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitaRepository(IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager, MyDbContext db, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
        }



        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {

            List<citaDTO> citaslst = _db.citas
                .Where(p => p.cod_verificacionID == codVerificacion && p.estado == true)
                .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();

            return citaslst;

        }

        public citas get(int Id, int medicoId)
        {

            try
            {

                citas cita = _db.citas
                    .Include(m => m.medicos).ThenInclude(hm => hm.horarios_medicos)
                    .FirstOrDefault(c => (c.ID == Id && c.medicosID == medicoId));

                return cita;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar acceder a la cita solicitada, por favor intente más tarde." + e.StackTrace);
            }
        }

        public async Task<List<citaDTO>> getCitasListAsync(int medicoId, DateTime? inicio = null,
                                                           DateTime? fin = null, bool? estado = null,
                                                           int? servicioId = null, int? seguroId = null)
        {

            try
            {

                List<citaDTO> citaslst = await _db.citas
                        .Include(c => c.medicos)
                        .ThenInclude(m => m.turnos)
                        .Include(c => c.medicos)
                        .ThenInclude(m => m.horarios_medicos)
                    .Where(p => p.medicosID == medicoId
                                && !p.deleted
                                && p.serviciosID == (servicioId == null ? p.serviciosID : servicioId.Value)
                                && p.segurosID == (seguroId == null ? p.segurosID : seguroId.Value)
                                && p.fecha_hora.Date >= (inicio == null ? p.fecha_hora.Date : inicio.Value.Date)
                                && p.fecha_hora.Date <= (fin == null ? p.fecha_hora.Date : fin.Value.Date)
                                && p.estado == (estado == null ? p.estado : estado.Value))
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToListAsync();

                citaslst.ForEach(citaDTO =>
                {
                    if (int.TryParse(citaDTO.medicosID.ToString(), out int _medicoID))
                    {
                        citaDTO.turno_paciente.cant_pacientes_adelante = getCantCitasPendientes(citaDTO.fecha_hora, _medicoID);
                        citaDTO.turno_paciente.ultima_entrada = getLastTurnByDate(citaDTO.fecha_hora, _medicoID);
                        citaDTO.turno_paciente.primera_entrada = getFirstTurnByDate(citaDTO.fecha_hora, _medicoID);
                    }
                });

                return citaslst;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar acceder a la información, por favor intente más tarde." + e.StackTrace);
            }
        }

        public DateTime? getFirstTurnByDate(DateTime fecha_hora, int medicoID)
        {
            var res = _db.citas
                         .Where(x => x.fecha_hora.Date == fecha_hora.Date
                                  && x.medicosID == medicoID
                                  && x.deleted)
                         .Min(c => c.deleted_date);

            return res;

        }

        public DateTime? getLastTurnByDate(DateTime fecha_hora, int medicoID)
        {
            var res = _db.citas
                         .Where(x => x.fecha_hora.Date == fecha_hora.Date
                                  && x.medicosID == medicoID
                                  && x.deleted)
                         .Max(c => c.deleted_date);

            return res;

        }

        private string getCV(MyIdentityUser user)
        {
            cod_verificacion codV = _db.cod_verificacion
                .Include("citas")
                .Where(x => x.citas.pacientes.MyIdentityUsers == user && x.citas.estado == true)
                .FirstOrDefault();

            return codV.value;
        }

        public void Add(citas entity)
        {
            _db.citas.Add(entity);
        }
        public bool Exist(medicos medico, string doc_identidad)
        {
            try
            {
                if (_db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.doc_identidad == doc_identidad && x.estado == true) != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public int getCantCitasPendientes(DateTime fecha_hora_cita, int medicosID)
        {
            var res = _db.citas.Where(x =>
                        !x.deleted
                        && x.fecha_hora.Date == fecha_hora_cita.Date
                        && x.medicosID == medicosID
                        && x.fecha_hora.TimeOfDay < fecha_hora_cita.TimeOfDay
                        ).Count();

            return res;
        }
        public void Remove(citas entity)
        {
            try
            {
                _db.citas.Remove(entity);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(citas entity)
        {
            try
            {
                _db.citas.Update(entity);

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
