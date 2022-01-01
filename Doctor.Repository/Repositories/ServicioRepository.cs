using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class ServicioRepository : IServicioRepository
    {

        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly UserManager<MyIdentityUser> _userManager;

        public ServicioRepository( 
            IMapper mapper,
            MyDbContext db)
        {
            _db = db;
             _mapper = mapper;
        }

        public async Task<List<servicio_coberturasDTO>> getAllByDoctorIdAsync(int medicoID)
        {

            try
            {
                List<servicio_coberturasDTO> r = await _db.servicios_medicos
                   .Where(x => x.medicosID == medicoID).Select(x => new servicio_coberturasDTO
                   {
                       ID = x.servicios.ID,
                       descrip = x.servicios.descrip,
                       coberturas = _mapper.Map<List<coberturaDTO>>( _db.cobertura_medicos.Include("seguros")
                      .Where(c => c.medicosID == x.medicosID && c.serviciosID == x.servicios.ID)
                   .ToList())
                   }).ToListAsync();
            return r;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<servicios> getByIdAsync(int? segurosID)
        {
            try
            {
                servicios servicio = await _db.servicios.FindAsync(segurosID);
                return servicio;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
