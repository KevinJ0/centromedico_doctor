using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class EspecialidadRepository : IEspecialidadRepository
    {

        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly IMedicoRepository _medicoRepo;

        public EspecialidadRepository(
            IMapper mapper,
            IMedicoRepository medicoRepo,
            MyDbContext db)
        {
            _medicoRepo = medicoRepo;
            _db = db;
            _mapper = mapper;
        }


        public List<especialidadDTO> get()
        {

            try
            {
                int medicoID = _medicoRepo.getMedicoIdAsync(null).Result;

                List<especialidadDTO> r = _db.especialidades_medicos.Include("especialidades")
                    .Where(es => es.medicosID == medicoID)
                   .Select(x => new especialidadDTO
                   {
                       ID = x.especialidadesID,
                       descrip = x.especialidades.descrip
                   }).ToList();
                return r;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void deleteEspecMedico(especialidades_medicos entity)
        {
            _db.especialidades_medicos.Remove(entity);
        }


        public List<especialidadDTO> getAll()
        {

            try
            {

                List<especialidadDTO> doctorEspec = get();

                List<especialidadDTO> r = _db.especialidades
                         .Select(x => new especialidadDTO
                         {
                             ID = x.ID,
                             descrip = x.descrip
                         }).ToList();

                r = r.Where(x => !doctorEspec.Any(em => em.ID == x.ID)).ToList();
                return r;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void addEspecMedico(especialidades_medicos entity)
        {
            _db.especialidades_medicos.Add(entity);

        }
    }
}
